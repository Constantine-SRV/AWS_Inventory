using Amazon;
using Amazon.ElasticFileSystem;
using Amazon.ElasticFileSystem.Model;
using Amazon.Runtime;
using Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    public class EFSService
    {
        private readonly AwsCredentials _credentials;

        public EFSService(AwsCredentials credentials)
        {
            _credentials = credentials;
        }

        public async Task<(List<EFS_FileSystem> fileSystems, List<EFS_AccessPoints> accessPoints)> GetEFSAsync(List<RegionEndpoint> accessibleRegions)
        {
            var fileSystems = new List<EFS_FileSystem>();
            var accessPoints = new List<EFS_AccessPoints>();

            foreach (var region in accessibleRegions)
            {
                Console.WriteLine($"Region: {region.SystemName}");
                var (regionFileSystems, regionAccessPoints) = await InventoryEFS(region);
                fileSystems.AddRange(regionFileSystems);
                accessPoints.AddRange(regionAccessPoints);
            }

            return (fileSystems, accessPoints);
        }

        private async Task<(List<EFS_FileSystem>, List<EFS_AccessPoints>)> InventoryEFS(RegionEndpoint region)
        {
            var fileSystems = new List<EFS_FileSystem>();
            var accessPoints = new List<EFS_AccessPoints>();
            var efsClient = new AmazonElasticFileSystemClient(new BasicAWSCredentials(_credentials.AccessKeyId, _credentials.SecretAccessKey), region);

            try
            {
                var describeFileSystemsResponse = await efsClient.DescribeFileSystemsAsync(new DescribeFileSystemsRequest());

                foreach (var fileSystem in describeFileSystemsResponse.FileSystems)
                {
                    var tagsResponse = await efsClient.ListTagsForResourceAsync(new ListTagsForResourceRequest
                    {
                        ResourceId = fileSystem.FileSystemId
                    });

                    var tags = tagsResponse.Tags.ToDictionary(tag => tag.Key.ToUpper(), tag => tag.Value);

                    var efsFileSystem = new EFS_FileSystem
                    {
                        Region = region.SystemName,
                        Name = tags.ContainsKey("NAME") ? tags["NAME"] : null,
                        FileSystemId = fileSystem.FileSystemId,
                        TotalSize = fileSystem.SizeInBytes.Value,
                        SizeInStandard = fileSystem.SizeInBytes.Value,  // We will just use the TotalSize here
                        CreationTime = fileSystem.CreationTime,
                        Tags = tags
                    };

                    fileSystems.Add(efsFileSystem);

                    var describeAccessPointsResponse = await efsClient.DescribeAccessPointsAsync(new DescribeAccessPointsRequest
                    {
                        FileSystemId = fileSystem.FileSystemId
                    });

                    foreach (var accessPoint in describeAccessPointsResponse.AccessPoints)
                    {
                        var accessPointTagsResponse = await efsClient.ListTagsForResourceAsync(new ListTagsForResourceRequest
                        {
                            ResourceId = accessPoint.AccessPointId
                        });

                        var accessPointTags = accessPointTagsResponse.Tags.ToDictionary(tag => tag.Key.ToUpper(), tag => tag.Value);

                        var efsAccessPoint = new EFS_AccessPoints
                        {
                            Region = region.SystemName,
                            Name = accessPointTags.ContainsKey("NAME") ? accessPointTags["NAME"] : null,
                            AccessPointId = accessPoint.AccessPointId,
                            FileSystemName = efsFileSystem.Name,
                            Path = accessPoint.RootDirectory?.Path,
                            Tags = accessPointTags
                        };

                        accessPoints.Add(efsAccessPoint);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"EFS Error: {e.Message}, Region: {region.SystemName}");
            }

            return (fileSystems, accessPoints);
        }
    }
}
