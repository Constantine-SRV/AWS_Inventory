using Amazon;
using Amazon.EC2;
using Amazon.EC2.Model;
using Amazon.Runtime;
using Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    public class ElasticIPService
    {
        private readonly AwsCredentials _credentials;

        public ElasticIPService(AwsCredentials credentials)
        {
            _credentials = credentials;
        }

        public async Task<List<ElasticIP>> GetElasticIPsAsync(List<RegionEndpoint> accessibleRegions)
        {
            var elasticIPs = new List<ElasticIP>();

            foreach (var region in accessibleRegions)
            {
                Console.WriteLine($"Region: {region.SystemName}");
                var ips = await InventoryElasticIPs(region);
                elasticIPs.AddRange(ips);
            }

            return elasticIPs;
        }

        private async Task<List<ElasticIP>> InventoryElasticIPs(RegionEndpoint region)
        {
            var elasticIPs = new List<ElasticIP>();
            var ec2Client = new AmazonEC2Client(new BasicAWSCredentials(_credentials.AccessKeyId, _credentials.SecretAccessKey), region);

            try
            {
                var describeAddressesResponse = await ec2Client.DescribeAddressesAsync(new DescribeAddressesRequest());

                foreach (var address in describeAddressesResponse.Addresses)
                {
                    string instanceName = null;
                    if (!string.IsNullOrEmpty(address.InstanceId))
                    {
                        instanceName = await GetInstanceName(ec2Client, address.InstanceId);
                    }

                    string natGatewayName = null;
                    if (!string.IsNullOrEmpty(address.AssociationId))
                    {
                        natGatewayName = await GetNatGatewayName(ec2Client, address.AllocationId);
                    }

                    var elasticIP = new ElasticIP
                    {
                        Region = region.SystemName,
                        AllocationId = address.AllocationId,
                        PublicIp = address.PublicIp,
                        PrivateIp = address.PrivateIpAddress,
                        AssociatedInstanceName = instanceName,
                        NatGatewayName = natGatewayName,
                        Tags = address.Tags.ToDictionary(tag => tag.Key.ToUpper(), tag => tag.Value)
                    };

                    elasticIPs.Add(elasticIP);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Elastic IP Error: " + e.Message);
            }

            return elasticIPs;
        }

        private async Task<string> GetInstanceName(AmazonEC2Client ec2Client, string instanceId)
        {
            var describeInstancesResponse = await ec2Client.DescribeInstancesAsync(new DescribeInstancesRequest
            {
                InstanceIds = new List<string> { instanceId }
            });

            var instance = describeInstancesResponse.Reservations.SelectMany(r => r.Instances).FirstOrDefault();
            return instance?.Tags.FirstOrDefault(t => t.Key == "Name")?.Value;
        }

        private async Task<string> GetNatGatewayName(AmazonEC2Client ec2Client, string allocationId)
        {
            var describeNatGatewaysResponse = await ec2Client.DescribeNatGatewaysAsync(new DescribeNatGatewaysRequest());

            foreach (var natGateway in describeNatGatewaysResponse.NatGateways)
            {
                var address = natGateway.NatGatewayAddresses.FirstOrDefault(addr => addr.AllocationId == allocationId);
                if (address != null)
                {
                    return natGateway.Tags.FirstOrDefault(t => t.Key == "Name")?.Value;
                }
            }

            return null;
        }
    }
}
