using Amazon;
using Amazon.IdentityManagement;
using Amazon.IdentityManagement.Model;
using Amazon.Runtime;
using AWS_Inventory.Data;
using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    public class InvIamRoleService
    {
        private readonly AwsCredentials _credentials;

        public InvIamRoleService(AwsCredentials credentials)
        {
            _credentials = credentials;
        }

        public async Task<List<InvIamRole>> GetIamRolesAsync()
        {
            var iamRoles = new List<InvIamRole>();
            var iamClient = new AmazonIdentityManagementServiceClient(new BasicAWSCredentials(_credentials.AccessKeyId, _credentials.SecretAccessKey));

            try
            {
                var response = await iamClient.ListRolesAsync();

                foreach (var role in response.Roles)
                {
                    var iamRole = new InvIamRole
                    {
                        Region = "global", // IAM is global
                        RoleName = role.RoleName,
                        RoleId = role.RoleId,
                        Arn = role.Arn,
                        Path = role.Path,
                        CreatedDate = role.CreateDate,
                        Tags = (await iamClient.ListRoleTagsAsync(new ListRoleTagsRequest { RoleName = role.RoleName })).Tags.ToDictionary(tag => tag.Key.ToUpper(), tag => tag.Value)
                    };

                    iamRoles.Add(iamRole);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("IAM Roles Error: " + e.Message);
            }

            return iamRoles;
        }
    }
}
