using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Amazon.Runtime;
using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AWS_Inventory.Data;

namespace Services
{
    public class AWSSecretsManagerService
    {
        private readonly AwsCredentials _credentials;

        public AWSSecretsManagerService(AwsCredentials credentials)
        {
            _credentials = credentials;
        }

        public async Task<List<AWSSecret>> GetSecretsAsync(List<RegionEndpoint> accessibleRegions)
        {
            var secrets = new List<AWSSecret>();

            foreach (var region in accessibleRegions)
            {
                Console.WriteLine($"Secrets Manager Region: {region.SystemName}");
                var regionSecrets = await InventorySecrets(region);
                secrets.AddRange(regionSecrets);
            }

            return secrets;
        }

        private async Task<List<AWSSecret>> InventorySecrets(RegionEndpoint region)
        {
            var secrets = new List<AWSSecret>();
            var secretsManagerClient = new AmazonSecretsManagerClient(new BasicAWSCredentials(_credentials.AccessKeyId, _credentials.SecretAccessKey), region);

            try
            {
                var request = new ListSecretsRequest();
                var response = await secretsManagerClient.ListSecretsAsync(request);

                foreach (var secretListEntry in response.SecretList)
                {
                    var secret = new AWSSecret
                    {
                        SecretName = secretListEntry.Name,
                        LastRetrieved = secretListEntry.LastAccessedDate,
                        Tags = secretListEntry.Tags.ToDictionary(tag => tag.Key.ToUpper(), tag => tag.Value)
                    };

                    secrets.Add(secret);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Secrets Manager Error: " + e.Message);
            }

            return secrets;
        }
    }
}
