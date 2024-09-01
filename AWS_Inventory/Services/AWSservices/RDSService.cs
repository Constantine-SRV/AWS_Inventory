using Amazon;
using Amazon.RDS;
using Amazon.RDS.Model;
using Amazon.Runtime;
using Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    public class RDSService
    {
        private readonly AwsCredentials _credentials;

        public RDSService(AwsCredentials credentials)
        {
            _credentials = credentials;
        }

        public async Task<List<RDSInstance>> GetRDSInstancesAsync(List<RegionEndpoint> accessibleRegions)
        {
            var rdsInstances = new List<RDSInstance>();

            foreach (var region in accessibleRegions)
            {
                Console.WriteLine($"Rds Region: {region.SystemName}");
                var instances = await InventoryRDS(region);
                rdsInstances.AddRange(instances);
            }

            return rdsInstances;
        }

        private async Task<List<RDSInstance>> InventoryRDS(RegionEndpoint region)
        {
            var rdsInstances = new List<RDSInstance>();
            var rdsClient = new AmazonRDSClient(new BasicAWSCredentials(_credentials.AccessKeyId, _credentials.SecretAccessKey), region);

            try
            {
                var request = new DescribeDBInstancesRequest();
                var response = await rdsClient.DescribeDBInstancesAsync(request);

                foreach (var dbInstance in response.DBInstances)
                {
                    var rdsInstance = new RDSInstance
                    {
                        Region = region.SystemName,
                        DBInstanceId = dbInstance.DBInstanceIdentifier,
                        Engine = dbInstance.Engine,
                        EngineVersion = dbInstance.EngineVersion,
                        VpcSecurityGroups = dbInstance.VpcSecurityGroups.Select(vsg => vsg.VpcSecurityGroupId).ToList(),
                        CreatedTime = dbInstance.InstanceCreateTime,
                        DBInstanceParameterGroup = dbInstance.DBParameterGroups.FirstOrDefault()?.DBParameterGroupName,
                        InstanceClass = dbInstance.DBInstanceClass,
                        MasterUsername = dbInstance.MasterUsername,
                        DbName = dbInstance.DBName,
                        Storage = dbInstance.AllocatedStorage,
                        ProvisionedIops = dbInstance.Iops,
                        StorageThroughput = dbInstance.MaxAllocatedStorage, // Placeholder
                        Tags = (await rdsClient.ListTagsForResourceAsync(new ListTagsForResourceRequest
                        {
                            ResourceName = dbInstance.DBInstanceArn
                        })).TagList.ToDictionary(tag => tag.Key.ToUpper(), tag => tag.Value)
                    };
                    rdsInstances.Add(rdsInstance);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("RDS Error: " + e.Message);
            }

            return rdsInstances;
        }
    }
}
