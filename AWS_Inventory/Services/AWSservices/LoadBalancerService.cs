using Amazon;
using Amazon.ElasticLoadBalancingV2;
using Amazon.ElasticLoadBalancingV2.Model;
using Amazon.Runtime;
using Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    public class LoadBalancerService
    {
        private readonly AwsCredentials _credentials;

        public LoadBalancerService(AwsCredentials credentials)
        {
            _credentials = credentials;
        }

        public async Task<List<ELBInstance>> GetLoadBalancersAsync(List<RegionEndpoint> accessibleRegions)
        {
            var loadBalancers = new List<ELBInstance>();

            foreach (var region in accessibleRegions)
            {
                Console.WriteLine($"Region: {region.SystemName}");
                var lbs = await InventoryLoadBalancers(region);
                loadBalancers.AddRange(lbs);
            }

            return loadBalancers;
        }

        private async Task<List<ELBInstance>> InventoryLoadBalancers(RegionEndpoint region)
        {
            var loadBalancers = new List<ELBInstance>();
            var elbClient = new AmazonElasticLoadBalancingV2Client(new BasicAWSCredentials(_credentials.AccessKeyId, _credentials.SecretAccessKey), region);

            try
            {
                var describeLoadBalancersResponse = await elbClient.DescribeLoadBalancersAsync(new DescribeLoadBalancersRequest());

                foreach (var loadBalancer in describeLoadBalancersResponse.LoadBalancers)
                {
                    try
                    {
                        var tagsResponse = await elbClient.DescribeTagsAsync(new DescribeTagsRequest
                        {
                            ResourceArns = new List<string> { loadBalancer.LoadBalancerArn }
                        });

                        var tags = new Dictionary<string, string>();
                        foreach (var tagDescription in tagsResponse.TagDescriptions)
                        {
                            foreach (var tag in tagDescription.Tags)
                            {
                                var upperKey = tag.Key.ToUpper();
                                if (!tags.ContainsKey(upperKey))
                                {
                                    tags[upperKey] = tag.Value;
                                }
                            }
                        }

                        var lb = new ELBInstance
                        {
                            Region = region.SystemName,
                            Name = loadBalancer.LoadBalancerName,
                            LoadBalancerArn = loadBalancer.LoadBalancerArn,
                            HostedZoneId = loadBalancer.CanonicalHostedZoneId,
                            CreatedTime = loadBalancer.CreatedTime,
                            Tags = tags
                        };

                        loadBalancers.Add(lb);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Load Balancer Error: {e.Message}, Load Balancer Name: {loadBalancer.LoadBalancerName}");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Load Balancer Error: {e.Message}, Region: {region.SystemName}");
            }

            return loadBalancers;
        }
    }
}
