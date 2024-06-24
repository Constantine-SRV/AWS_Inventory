namespace Data
{
    public class ELBInstance
    {
        public string Region { get; set; }
        public string Name { get; set; }
        public string LoadBalancerArn { get; set; }
        public string HostedZoneId { get; set; }
        public DateTime CreatedTime { get; set; }
        public Dictionary<string, string> Tags { get; set; }
    }
}
