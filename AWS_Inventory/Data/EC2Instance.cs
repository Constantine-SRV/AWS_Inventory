namespace Data
{
    public class EC2Instance
    {
        public string Region { get; set; }
        public string InstanceId { get; set; }
        public string InstanceType { get; set; }
        public string PublicDnsName { get; set; }
        public string KeyName { get; set; }
        public DateTime LaunchTime { get; set; }
        public string State { get; set; }
        public string PrivateIpAddress { get; set; }
        public string PublicIpAddress { get; set; }
        public string AvailabilityZone { get; set; }
        public string SubnetId { get; set; }
        public string ImageId { get; set; }
        public string Platform { get; set; }
        public DateTime? AttachTime { get; set; }
        public Dictionary<string, string> Tags { get; set; }
    }
}
