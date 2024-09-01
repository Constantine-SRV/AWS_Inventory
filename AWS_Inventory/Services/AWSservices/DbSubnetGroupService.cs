using Amazon;
using Amazon.RDS;
using Amazon.RDS.Model;
using Amazon.Runtime;
using AWS_Inventory.Data;
using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    public class DbSubnetGroupService
    {
        private readonly AwsCredentials _credentials;

        public DbSubnetGroupService(AwsCredentials credentials)
        {
            _credentials = credentials;
        }

        public async Task<List<DbSubnetGroup>> GetDbSubnetGroupsAsync(List<RegionEndpoint> accessibleRegions)
        {
            var dbSubnetGroups = new List<DbSubnetGroup>();

            foreach (var region in accessibleRegions)
            {
                Console.WriteLine($"RDS Region: {region.SystemName}");
                var regionDbSubnetGroups = await InventoryDbSubnetGroups(region);
                dbSubnetGroups.AddRange(regionDbSubnetGroups);
            }

            return dbSubnetGroups;
        }

        private async Task<List<DbSubnetGroup>> InventoryDbSubnetGroups(RegionEndpoint region)
        {
            var dbSubnetGroups = new List<DbSubnetGroup>();
            var rdsClient = new AmazonRDSClient(new BasicAWSCredentials(_credentials.AccessKeyId, _credentials.SecretAccessKey), region);

            try
            {
                var request = new DescribeDBSubnetGroupsRequest();
                var response = await rdsClient.DescribeDBSubnetGroupsAsync(request);

                foreach (var subnetGroup in response.DBSubnetGroups)
                {
                    var dbSubnetGroup = new DbSubnetGroup
                    {
                        Name = subnetGroup.DBSubnetGroupName,
                        Description = subnetGroup.DBSubnetGroupDescription,
                        SubnetIds = subnetGroup.Subnets.Select(s => s.SubnetIdentifier).ToList(),
                        Tags = (await rdsClient.ListTagsForResourceAsync(new ListTagsForResourceRequest
                        {
                            ResourceName = subnetGroup.DBSubnetGroupArn
                        })).TagList.ToDictionary(tag => tag.Key.ToUpper(), tag => tag.Value)
                    };

                    dbSubnetGroups.Add(dbSubnetGroup);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"RDS Subnet Group Error in region {region.SystemName}: " + e.Message);
            }

            return dbSubnetGroups;
        }
    }
}
