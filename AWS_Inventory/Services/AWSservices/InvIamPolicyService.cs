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
    public class InvIamPolicyService
    {
        private readonly AwsCredentials _credentials;

        public InvIamPolicyService(AwsCredentials credentials)
        {
            _credentials = credentials;
        }

        public async Task<List<InvIamPolicy>> GetIamPoliciesAsync()
        {
            var iamPolicies = new List<InvIamPolicy>();
            var iamClient = new AmazonIdentityManagementServiceClient(new BasicAWSCredentials(_credentials.AccessKeyId, _credentials.SecretAccessKey));

            try
            {
                var response = await iamClient.ListPoliciesAsync();

                foreach (var policy in response.Policies)
                {
                    var iamPolicy = new InvIamPolicy
                    {
                        Region = "global", // IAM is global
                        PolicyName = policy.PolicyName,
                        PolicyId = policy.PolicyId,
                        Arn = policy.Arn,
                        CreatedDate = policy.CreateDate,
                        IsAttachable = policy.IsAttachable,
                        DefaultVersionId = policy.DefaultVersionId,
                        Tags = (await iamClient.ListPolicyTagsAsync(new ListPolicyTagsRequest { PolicyArn = policy.Arn })).Tags.ToDictionary(tag => tag.Key.ToUpper(), tag => tag.Value)
                    };

                    iamPolicies.Add(iamPolicy);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("IAM Policies Error: " + e.Message);
            }

            return iamPolicies;
        }
    }
}
