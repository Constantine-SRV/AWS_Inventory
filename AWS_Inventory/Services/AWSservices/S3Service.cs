using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Runtime;
using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    public class S3Service
    {
        private readonly AwsCredentials _credentials;

        public S3Service(AwsCredentials credentials)
        {
            _credentials = credentials;
        }

        public async Task<List<S3>> GetS3BucketsAsync()
        {
            var s3Buckets = new List<S3>();
            var s3Client = new AmazonS3Client(new BasicAWSCredentials(_credentials.AccessKeyId, _credentials.SecretAccessKey), RegionEndpoint.USEast1); // S3 is a global service.

            try
            {
                var listBucketsResponse = await s3Client.ListBucketsAsync();

                foreach (var bucket in listBucketsResponse.Buckets)
                {
                    var regionResponse = await s3Client.GetBucketLocationAsync(new GetBucketLocationRequest
                    {
                        BucketName = bucket.BucketName
                    });

                    var bucketRegion = GetRegionEndpoint(regionResponse.Location.Value);
                    var bucketTags = await GetBucketTagsAsync(s3Client, bucket.BucketName);

                    var s3Bucket = new S3
                    {
                        Region = bucketRegion.SystemName,
                        Name = bucket.BucketName,
                        CreationDate = bucket.CreationDate,
                        Tags = bucketTags
                    };

                    s3Buckets.Add(s3Bucket);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("S3 Error: " + e.Message);
            }

            return s3Buckets;
        }

        private async Task<Dictionary<string, string>> GetBucketTagsAsync(AmazonS3Client s3Client, string bucketName)
        {
            var tags = new Dictionary<string, string>();

            try
            {
                var getTaggingResponse = await s3Client.GetBucketTaggingAsync(new GetBucketTaggingRequest
                {
                    BucketName = bucketName
                });

                tags = getTaggingResponse.TagSet
                    .ToDictionary(tag => tag.Key.ToUpper(), tag => tag.Value);
            }
            catch (AmazonS3Exception e) when (e.ErrorCode == "NoSuchTagSet")
            {
                // No tags set for the bucket
            }

            return tags;
        }

        private RegionEndpoint GetRegionEndpoint(string location)
        {
            switch (location)
            {
                case "EU":
                    return RegionEndpoint.EUWest1;
                case "us-west-1":
                    return RegionEndpoint.USWest1;
                case "us-west-2":
                    return RegionEndpoint.USWest2;
                case "sa-east-1":
                    return RegionEndpoint.SAEast1;
                case "ap-northeast-1":
                    return RegionEndpoint.APNortheast1;
                case "ap-northeast-2":
                    return RegionEndpoint.APNortheast2;
                case "ap-south-1":
                    return RegionEndpoint.APSouth1;
                case "ap-southeast-1":
                    return RegionEndpoint.APSoutheast1;
                case "ap-southeast-2":
                    return RegionEndpoint.APSoutheast2;
                case "cn-north-1":
                    return RegionEndpoint.CNNorth1;
                case "cn-northwest-1":
                    return RegionEndpoint.CNNorthWest1;
                case "eu-central-1":
                    return RegionEndpoint.EUCentral1;
                case "us-gov-west-1":
                    return RegionEndpoint.USGovCloudWest1;
                default:
                    return RegionEndpoint.USEast1;
            }
        }
    }
}
