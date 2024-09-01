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
    public class SecurityGroupService
    {
        private readonly AwsCredentials _credentials;

        public SecurityGroupService(AwsCredentials credentials)
        {
            _credentials = credentials;
        }

        public async Task<(List<InvSecurityGroup>, List<InvSecurityGroupRule>)> GetSecurityGroupsAsync(List<RegionEndpoint> accessibleRegions)
        {
            var securityGroups = new List<InvSecurityGroup>();
            var securityGroupRules = new List<InvSecurityGroupRule>();

            foreach (var region in accessibleRegions)
            {
                Console.WriteLine($"EC2 Region: {region.SystemName}");
                var (regionSecurityGroups, regionSecurityGroupRules) = await InventorySecurityGroups(region);
                securityGroups.AddRange(regionSecurityGroups);
                securityGroupRules.AddRange(regionSecurityGroupRules);
            }

            return (securityGroups, securityGroupRules);
        }

        private async Task<(List<InvSecurityGroup>, List<InvSecurityGroupRule>)> InventorySecurityGroups(RegionEndpoint region)
        {
            var securityGroups = new List<InvSecurityGroup>();
            var securityGroupRules = new List<InvSecurityGroupRule>();
            var ec2Client = new AmazonEC2Client(new BasicAWSCredentials(_credentials.AccessKeyId, _credentials.SecretAccessKey), region);

            try
            {
                var describeRequest = new DescribeSecurityGroupsRequest();
                var describeResponse = await ec2Client.DescribeSecurityGroupsAsync(describeRequest);

                foreach (var group in describeResponse.SecurityGroups)
                {
                    // Сбор информации о Security Group
                    var securityGroup = new InvSecurityGroup
                    {
                        Name = group.GroupName,
                        GroupId = group.GroupId,
                        Description = group.Description,
                        VpcId = group.VpcId,
                        Owner = group.OwnerId,
                        Region = region.SystemName,  
                        Tags = new Dictionary<string, string>()
                    };

                    foreach (var tag in group.Tags)
                    {
                        securityGroup.Tags[tag.Key.ToUpper()] = tag.Value;
                    }

                    securityGroups.Add(securityGroup);

                    //  Inbound
                    var sources = group.IpPermissions
                        .SelectMany(rule => rule.IpRanges.Select(r => r)
                            .Concat(rule.Ipv6Ranges.Select(r => r.CidrIpv6))
                            .Concat(rule.PrefixListIds.Select(p => p.Description)))
                        .ToList();

                    foreach (var rule in group.IpPermissions)
                    {
                        var securityGroupRule = new InvSecurityGroupRule
                        {
                            SecurityGroupId = group.GroupId,
                            Direction = "Inbound",
                            SourceOrDestination = string.Join(";", sources),
                            Protocol = rule.IpProtocol,
                            FromPort = rule.FromPort,
                            ToPort = rule.ToPort
                        };

                        securityGroupRules.Add(securityGroupRule);
                    }

                    //  Outbound
                    var destinations = group.IpPermissionsEgress
                        .SelectMany(rule => rule.IpRanges.Select(r => r)
                            .Concat(rule.Ipv6Ranges.Select(r => r.CidrIpv6))
                            .Concat(rule.PrefixListIds.Select(p => p.Description)))
                        .ToList();

                    foreach (var rule in group.IpPermissionsEgress)
                    {
                        var securityGroupRule = new InvSecurityGroupRule
                        {
                            SecurityGroupId = group.GroupId,
                            Direction = "Outbound",
                            SourceOrDestination = string.Join(";", destinations),
                            Protocol = rule.IpProtocol,
                            FromPort = rule.FromPort,
                            ToPort = rule.ToPort
                        };

                        securityGroupRules.Add(securityGroupRule);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("EC2 Security Group Error: " + e.Message);
            }

            return (securityGroups, securityGroupRules);
        }
    }
}
