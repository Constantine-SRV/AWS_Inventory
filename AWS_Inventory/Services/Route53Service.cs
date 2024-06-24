using Amazon;
using Amazon.Route53;
using Amazon.Route53.Model;
using Amazon.Runtime;
using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    public class Route53Service
    {
        private readonly AwsCredentials _credentials;

        public Route53Service(AwsCredentials credentials)
        {
            _credentials = credentials;
        }

        public async Task<List<Route53HostedZone>> GetHostedZonesAsync()
        {
            var hostedZones = new List<Route53HostedZone>();
            var route53Client = new AmazonRoute53Client(new BasicAWSCredentials(_credentials.AccessKeyId, _credentials.SecretAccessKey), RegionEndpoint.USEast1); // Route 53 is a global service.

            try
            {
                var listHostedZonesResponse = await route53Client.ListHostedZonesAsync(new ListHostedZonesRequest());

                foreach (var hostedZone in listHostedZonesResponse.HostedZones)
                {
                    try
                    {
                        Console.WriteLine($"Hosted Zone Name: {hostedZone.Name}, Hosted Zone ID: {hostedZone.Id}");
                        var zoneId = hostedZone.Id.Replace("/hostedzone/", ""); // Remove the /hostedzone/ prefix

                        var getHostedZoneResponse = await route53Client.GetHostedZoneAsync(new GetHostedZoneRequest
                        {
                            Id = zoneId
                        });

                        var route53HostedZone = new Route53HostedZone
                        {
                            Id = hostedZone.Id,
                            Name = hostedZone.Name,
                            CallerReference = hostedZone.CallerReference,
                            PrivateZone = getHostedZoneResponse.HostedZone.Config.PrivateZone,
                            ResourceRecordSetCount = getHostedZoneResponse.HostedZone.ResourceRecordSetCount
                        };

                        hostedZones.Add(route53HostedZone);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Route 53 Error: {e.Message}, Hosted Zone Name: {hostedZone.Name}, Hosted Zone ID: {hostedZone.Id}");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Route 53 Error: " + e.Message);
            }

            return hostedZones;
        }
    }
}
