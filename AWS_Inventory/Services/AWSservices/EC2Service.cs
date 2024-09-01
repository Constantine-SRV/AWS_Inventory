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
    public class EC2Service
    {
        private readonly AwsCredentials _credentials;

        public EC2Service(AwsCredentials credentials)
        {
            _credentials = credentials;
        }

        public async Task<List<EC2Instance>> GetEC2InstancesAsync(List<RegionEndpoint> accessibleRegions)
        {
            var ec2Instances = new List<EC2Instance>();

            foreach (var region in accessibleRegions)
            {
                Console.WriteLine($"EC2 Region: {region.SystemName}");
                var instances = await InventoryEC2(region);
                ec2Instances.AddRange(instances);
            }

            return ec2Instances;
        }

        private async Task<List<EC2Instance>> InventoryEC2(RegionEndpoint region)
        {
            var ec2Instances = new List<EC2Instance>();
            var ec2Client = new AmazonEC2Client(new BasicAWSCredentials(_credentials.AccessKeyId, _credentials.SecretAccessKey), region);

            try
            {
                var request = new DescribeInstancesRequest();
                var response = await ec2Client.DescribeInstancesAsync(request);

                foreach (var reservation in response.Reservations)
                {
                    foreach (var instance in reservation.Instances)
                    {
                        var ec2Instance = new EC2Instance
                        {
                            Region = region.SystemName,
                            InstanceId = instance.InstanceId,
                            InstanceType = instance.InstanceType,
                            PublicDnsName = instance.PublicDnsName,
                            KeyName = instance.KeyName,
                            LaunchTime = instance.LaunchTime,
                            State = instance.State.Name.Value,
                            PrivateIpAddress = instance.PrivateIpAddress,
                            PublicIpAddress = instance.PublicIpAddress,
                            AvailabilityZone = instance.Placement.AvailabilityZone,
                            SubnetId = instance.SubnetId,
                            ImageId = await GetAmiName(ec2Client, instance.ImageId),
                            Platform = instance.PlatformDetails,
                            AttachTime = instance.BlockDeviceMappings.FirstOrDefault()?.Ebs.AttachTime,
                            Tags = instance.Tags.ToDictionary(tag => tag.Key.ToUpper(), tag => tag.Value)
                        };
                        ec2Instances.Add(ec2Instance);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("EC2 Error: " + e.Message);
            }

            return ec2Instances;
        }

        private async Task<string> GetAmiName(AmazonEC2Client ec2Client, string imageId)
        {
            try
            {
                var request = new DescribeImagesRequest
                {
                    ImageIds = new List<string> { imageId }
                };
                var response = await ec2Client.DescribeImagesAsync(request);
                return response.Images.FirstOrDefault()?.Name ?? "Unknown";
            }
            catch
            {
                return "Unknown";
            }
        }
    }
}
