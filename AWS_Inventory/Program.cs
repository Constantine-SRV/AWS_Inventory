using Amazon.S3.Model;
using Amazon.SecretsManager;
//using AWS_Inventory.Services;
using Data;
using Services;
using System;
using System.Threading.Tasks;

namespace AwsInventoryApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var credentialsFilePath = @"C:\!\aws.json";
            if (args.Length > 0)
            {
                 credentialsFilePath = args[0];
            }
            AwsCredentials credentials;
            try
            {
                credentials = AwsCredentials.LoadFromFile(credentialsFilePath);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error loading credentials: {e.Message}");
                return;
            }

            var excelService = new ExcelService();
            var regionService = new AwsRegionService(credentials);
            var accessibleRegions = await regionService.GetAccessibleRegionsAsync();

            var ec2Service = new EC2Service(credentials);
            var rdsService = new RDSService(credentials);
            var ecsService = new ECSService(credentials);
            var elasticIPService = new ElasticIPService(credentials);
            var loadBalancerService = new LoadBalancerService(credentials);
            var efsService = new EFSService(credentials);
            var route53Service = new Route53Service(credentials);
            var s3Service = new S3Service(credentials);
            var secretsManagerService = new AWSSecretsManagerService(credentials);
            var dbSubnetGroupService = new DbSubnetGroupService(credentials);
            var securityGroupService = new SecurityGroupService(credentials);
            var launchTemplateService = new InvLaunchTemplateService(credentials);
            var iamUserService = new InvIamUserService(credentials);
            var iamRoleService = new InvIamRoleService(credentials);
            var iamPolicyService = new InvIamPolicyService(credentials);
            var ec2Instances = await ec2Service.GetEC2InstancesAsync(accessibleRegions);
            var rdsInstances = await rdsService.GetRDSInstancesAsync(accessibleRegions);
            var (ecsInstances, ecsClusters, ecsServices) = await ecsService.GetECSInstancesAsync(accessibleRegions);
            var elasticIPs = await elasticIPService.GetElasticIPsAsync(accessibleRegions);
            var loadBalancers = await loadBalancerService.GetLoadBalancersAsync(accessibleRegions);
            var (efsFileSystems, efsAccessPoints) = await efsService.GetEFSAsync(accessibleRegions);
            var hostedZones = await route53Service.GetHostedZonesAsync();
            var s3Buckets = await s3Service.GetS3BucketsAsync();
            var awsSecrets = await secretsManagerService.GetSecretsAsync(accessibleRegions);
            var dbSubnetGroups = await dbSubnetGroupService.GetDbSubnetGroupsAsync(accessibleRegions);
            var (securityGroups, securityGroupRules) = await securityGroupService.GetSecurityGroupsAsync(accessibleRegions);
            var launchTemplates = await launchTemplateService.GetLaunchTemplatesAsync(accessibleRegions);
            var iamUsers = await iamUserService.GetIamUsersAsync();
            var iamRoles = await iamRoleService.GetIamRolesAsync();
            var iamPolicies = await iamPolicyService.GetIamPoliciesAsync();

            await excelService.SaveToExcelAsync(ec2Instances, $"EC2Instances_{DateTime.Now:yyyy-MM-dd-HHmmss}.xlsx");
            await excelService.SaveToExcelAsync(rdsInstances, $"RDSInstances_{DateTime.Now:yyyy-MM-dd-HHmmss}.xlsx");
            await excelService.SaveToExcelAsync(ecsInstances, $"ECSInstances_{DateTime.Now:yyyy-MM-dd-HHmmss}.xlsx");
            await excelService.SaveToExcelAsync(ecsClusters, $"ECSClusters_{DateTime.Now:yyyy-MM-dd-HHmmss}.xlsx");
            await excelService.SaveToExcelAsync(ecsServices, $"ECSServices_{DateTime.Now:yyyy-MM-dd-HHmmss}.xlsx");
            await excelService.SaveToExcelAsync(elasticIPs, $"ElasticIPs_{DateTime.Now:yyyy-MM-dd-HHmmss}.xlsx");
            await excelService.SaveToExcelAsync(loadBalancers, $"LoadBalancers_{DateTime.Now:yyyy-MM-dd-HHmmss}.xlsx");
            await excelService.SaveToExcelAsync(efsFileSystems, $"EFSFileSystems_{DateTime.Now:yyyy-MM-dd-HHmmss}.xlsx");
            await excelService.SaveToExcelAsync(efsAccessPoints, $"EFSAccessPoints_{DateTime.Now:yyyy-MM-dd-HHmmss}.xlsx");
            await excelService.SaveToExcelAsync(hostedZones, $"Route53HostedZones_{DateTime.Now:yyyy-MM-dd-HHmmss}.xlsx");
            await excelService.SaveToExcelAsync(s3Buckets, $"S3Buckets_{DateTime.Now:yyyy-MM-dd-HHmmss}.xlsx");
            await excelService.SaveToExcelAsync(awsSecrets, $"awsSecrets{DateTime.Now:yyyy-MM-dd-HHmmss}.xlsx");
            await excelService.SaveToExcelAsync(dbSubnetGroups, $"DbSubnetGroups_{DateTime.Now:yyyy-MM-dd-HHmmss}.xlsx");
            await excelService.SaveToExcelAsync(securityGroups, $"SecurityGroups_{DateTime.Now:yyyy-MM-dd-HHmmss}.xlsx");
            await excelService.SaveToExcelAsync(securityGroupRules, $"SecurityGroupRules_{DateTime.Now:yyyy-MM-dd-HHmmss}.xlsx");
            await excelService.SaveToExcelAsync(launchTemplates, $"LaunchTemplates_{DateTime.Now:yyyy-MM-dd-HHmmss}.xlsx");
            await excelService.SaveToExcelAsync(iamUsers, $"IamUsers_{DateTime.Now:yyyy-MM-dd-HHmmss}.xlsx");
            await excelService.SaveToExcelAsync(iamRoles, $"IamRoles_{DateTime.Now:yyyy-MM-dd-HHmmss}.xlsx");
            await excelService.SaveToExcelAsync(iamPolicies, $"IamPolicies_{DateTime.Now:yyyy-MM-dd-HHmmss}.xlsx");





        }
    }
}
