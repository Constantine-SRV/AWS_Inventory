namespace Data
{
    public class Route53HostedZone
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string CallerReference { get; set; }
        public bool PrivateZone { get; set; }
        public long ResourceRecordSetCount { get; set; }
    }
}
