using Amazon;
using Amazon.EC2;
using Amazon.EC2.Model;
using Amazon.Runtime;
using AWS_Inventory.Data;
using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    public class InvLaunchTemplateService
    {
        private readonly AwsCredentials _credentials;

        public InvLaunchTemplateService(AwsCredentials credentials)
        {
            _credentials = credentials;
        }

        public async Task<List<InvLaunchTemplate>> GetLaunchTemplatesAsync(List<RegionEndpoint> accessibleRegions)
        {
            var launchTemplates = new List<InvLaunchTemplate>();

            foreach (var region in accessibleRegions)
            {
                Console.WriteLine($"EC2 Region: {region.SystemName}");
                var regionLaunchTemplates = await InventoryLaunchTemplates(region);
                launchTemplates.AddRange(regionLaunchTemplates);
            }

            return launchTemplates;
        }

        private async Task<List<InvLaunchTemplate>> InventoryLaunchTemplates(RegionEndpoint region)
        {
            var launchTemplates = new List<InvLaunchTemplate>();
            var ec2Client = new AmazonEC2Client(new BasicAWSCredentials(_credentials.AccessKeyId, _credentials.SecretAccessKey), region);

            try
            {
                var describeRequest = new DescribeLaunchTemplatesRequest();
                var describeResponse = await ec2Client.DescribeLaunchTemplatesAsync(describeRequest);

                foreach (var template in describeResponse.LaunchTemplates)
                {
                    var launchTemplate = new InvLaunchTemplate
                    {
                        Region = region.SystemName,  // Перемещено на первое место
                        TemplateId = template.LaunchTemplateId,
                        TemplateName = template.LaunchTemplateName,
                        DefaultVersion = template.DefaultVersionNumber.ToString(),
                        LatestVersion = template.LatestVersionNumber.ToString(),
                        CreatedBy = template.CreatedBy,
                        CreatedDate = template.CreateTime,
                        Tags = (await ec2Client.DescribeTagsAsync(new DescribeTagsRequest
                        {
                            Filters = new List<Filter>
                            {
                                new Filter
                                {
                                    Name = "resource-id",
                                    Values = new List<string> { template.LaunchTemplateId }
                                }
                            }
                        })).Tags.ToDictionary(tag => tag.Key.ToUpper(), tag => tag.Value)
                    };

                    launchTemplates.Add(launchTemplate);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Launch Template Error in region {region.SystemName}: " + e.Message);
            }

            return launchTemplates;
        }
    }
}
