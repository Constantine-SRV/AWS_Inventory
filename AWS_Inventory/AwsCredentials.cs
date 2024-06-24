using System;
using System.IO;
using System.Text.Json;

namespace Data
{
    public class AwsCredentials
    {
        public string AccessKeyId { get; set; }
        public string SecretAccessKey { get; set; }

        public static AwsCredentials LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Credentials file not found: {filePath}");
            }
            var json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<AwsCredentials>(json);
        }
    }
}
