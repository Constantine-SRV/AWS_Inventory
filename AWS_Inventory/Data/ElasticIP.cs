namespace Data
{
    public class ElasticIP
    {
        public string Region { get; set; }
        public string AllocationId { get; set; }
        public string PublicIp { get; set; }
        public string PrivateIp { get; set; }
        public string AssociatedInstanceName { get; set; }
        public string NatGatewayName { get; set; }
        public Dictionary<string, string> Tags { get; set; }
    }
}
