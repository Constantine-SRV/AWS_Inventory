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
    public class InvIamUserService
    {
        private readonly AwsCredentials _credentials;

        public InvIamUserService(AwsCredentials credentials)
        {
            _credentials = credentials;
        }

        public async Task<List<InvIamUser>> GetIamUsersAsync()
        {
            var iamUsers = new List<InvIamUser>();
            var iamClient = new AmazonIdentityManagementServiceClient(new BasicAWSCredentials(_credentials.AccessKeyId, _credentials.SecretAccessKey));

            try
            {
                var response = await iamClient.ListUsersAsync();

                foreach (var user in response.Users)
                {
                    string passwordLastUsedString = "";
                    if (user.PasswordLastUsed != null)
                    {
                        passwordLastUsedString = user.PasswordLastUsed.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    var iamUser = new InvIamUser
                    {
                        Region = "global", // IAM is global
                        UserName = user.UserName,
                        UserId = user.UserId,
                        CreatedDate = user.CreateDate,
                        Arn = user.Arn,
                        Path = user.Path,
                        PasswordLastUsed = passwordLastUsedString,
                        Tags = (await iamClient.ListUserTagsAsync(new ListUserTagsRequest { UserName = user.UserName })).Tags.ToDictionary(tag => tag.Key.ToUpper(), tag => tag.Value)
                    };

                    iamUsers.Add(iamUser);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("IAM Users Error: " + e.Message);
            }

            return iamUsers;
        }
    }
}
