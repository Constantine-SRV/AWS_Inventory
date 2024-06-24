using Amazon;
using Amazon.ECS;
using Amazon.ECS.Model;
using Amazon.Runtime;
using Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    public class ECSService
    {
        private readonly AwsCredentials _credentials;

        public ECSService(AwsCredentials credentials)
        {
            _credentials = credentials;
        }

        public async Task<(List<ECSInstance> allInstances, List<ECSCluster> clusters, List<ECSServiceInstance> services)> GetECSInstancesAsync(List<RegionEndpoint> accessibleRegions)
        {
            var allInstances = new List<ECSInstance>();
            var clusters = new List<ECSCluster>();
            var services = new List<ECSServiceInstance>();

            foreach (var region in accessibleRegions)
            {
                Console.WriteLine($"Region: {region.SystemName}");
                var (regionAllInstances, regionClusters, regionServices) = await InventoryECS(region);
                allInstances.AddRange(regionAllInstances);
                clusters.AddRange(regionClusters);
                services.AddRange(regionServices);
            }

            return (allInstances, clusters, services);
        }

        private async Task<(List<ECSInstance> allInstances, List<ECSCluster> clusters, List<ECSServiceInstance> services)> InventoryECS(RegionEndpoint region)
        {
            var allInstances = new List<ECSInstance>();
            var clusters = new List<ECSCluster>();
            var services = new List<ECSServiceInstance>();
            var ecsClient = new AmazonECSClient(new BasicAWSCredentials(_credentials.AccessKeyId, _credentials.SecretAccessKey), region);

            try
            {
                var listClustersResponse = await ecsClient.ListClustersAsync(new ListClustersRequest());
                var clusterArns = listClustersResponse.ClusterArns;

                foreach (var clusterArn in clusterArns)
                {
                    var describeClustersResponse = await ecsClient.DescribeClustersAsync(new DescribeClustersRequest
                    {
                        Clusters = new List<string> { clusterArn }
                    });

                    var cluster = describeClustersResponse.Clusters.FirstOrDefault();
                    if (cluster == null) continue;

                    var clusterTags = (await ecsClient.ListTagsForResourceAsync(new ListTagsForResourceRequest
                    {
                        ResourceArn = cluster.ClusterArn
                    })).Tags.ToDictionary(tag => tag.Key.ToUpper(), tag => tag.Value);

                    var ecsCluster = new ECSCluster
                    {
                        Region = region.SystemName,
                        ClusterName = cluster.ClusterName,
                        ClusterArn = cluster.ClusterArn,
                        Tags = clusterTags
                    };
                    clusters.Add(ecsCluster);

                    var listServicesResponse = await ecsClient.ListServicesAsync(new ListServicesRequest
                    {
                        Cluster = cluster.ClusterArn
                    });
                    var serviceArns = listServicesResponse.ServiceArns;

                    foreach (var serviceArn in serviceArns)
                    {
                        var describeServicesResponse = await ecsClient.DescribeServicesAsync(new DescribeServicesRequest
                        {
                            Cluster = cluster.ClusterArn,
                            Services = new List<string> { serviceArn }
                        });

                        var service = describeServicesResponse.Services.FirstOrDefault();
                        if (service == null) continue;

                        var serviceTags = (await ecsClient.ListTagsForResourceAsync(new ListTagsForResourceRequest
                        {
                            ResourceArn = service.ServiceArn
                        })).Tags.ToDictionary(tag => tag.Key.ToUpper(), tag => tag.Value);

                        var targetGroups = string.Join(";", service.LoadBalancers.Select(lb => lb.TargetGroupArn));
                        var loadBalancers = string.Join(";", service.LoadBalancers.Select(lb => lb.LoadBalancerName));

                        var ecsServiceInstance = new ECSServiceInstance
                        {
                            Region = region.SystemName,
                            ClusterName = cluster.ClusterName,
                            ServiceName = service.ServiceName,
                            ServiceArn = service.ServiceArn,
                            TargetGroups = targetGroups,
                            LoadBalancers = loadBalancers,
                            Tags = serviceTags
                        };
                        services.Add(ecsServiceInstance);

                        var allTags = MergeTags(clusterTags, serviceTags);

                        var ecsInstance = new ECSInstance
                        {
                            Region = region.SystemName,
                            ClusterName = cluster.ClusterName,
                            ServiceName = service.ServiceName,
                            TargetGroups = targetGroups,
                            LoadBalancers = loadBalancers,
                            Tags = allTags
                        };
                        allInstances.Add(ecsInstance);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("ECS Error: " + e.Message);
            }

            return (allInstances, clusters, services);
        }

        private Dictionary<string, string> MergeTags(Dictionary<string, string> clusterTags, Dictionary<string, string> serviceTags)
        {
            var allTags = new Dictionary<string, string>(clusterTags);

            foreach (var tag in serviceTags)
            {
                if (!allTags.ContainsKey(tag.Key))
                {
                    allTags.Add(tag.Key, tag.Value);
                }
                else
                {
                    // Optionally, handle duplicate keys if necessary
                    // For now, we just keep the existing key from clusterTags
                }
            }

            return allTags;
        }
    }
}
