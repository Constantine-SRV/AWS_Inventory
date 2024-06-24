using Amazon;
using Amazon.EC2;
using Amazon.EC2.Model;
using Amazon.Runtime;
using Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public class AwsRegionService
    {
        private readonly AwsCredentials _credentials;

        public AwsRegionService(AwsCredentials credentials)
        {
            _credentials = credentials;
        }

        public async Task<List<RegionEndpoint>> GetAccessibleRegionsAsync()
        {
            var accessibleRegions = new List<RegionEndpoint>();

            foreach (var region in RegionEndpoint.EnumerableAllRegions)
            {
                Console.WriteLine($"Region checking: {region.SystemName}");
                if (await IsRegionAccessible(region))
                {
                    accessibleRegions.Add(region);
                }
                else
                {
                    Console.WriteLine($"Region: {region.SystemName} is not accessible.");
                }
            }

            return accessibleRegions;
        }

        private async Task<bool> IsRegionAccessible(RegionEndpoint region)
        {
            try
            {
                var ec2Client = new AmazonEC2Client(new BasicAWSCredentials(_credentials.AccessKeyId, _credentials.SecretAccessKey), region);
                await ec2Client.DescribeRegionsAsync(new DescribeRegionsRequest());
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
