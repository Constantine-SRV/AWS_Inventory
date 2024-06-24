namespace Data
{
    public class RDSInstance
    {
        public string Region { get; set; }
        public string DBInstanceId { get; set; }
        public string Engine { get; set; }
        public string EngineVersion { get; set; }
        public List<string> VpcSecurityGroups { get; set; }
        public DateTime? CreatedTime { get; set; }
        public string DBInstanceParameterGroup { get; set; }
        public string InstanceClass { get; set; }
        public string MasterUsername { get; set; }
        public string DbName { get; set; }
        public int Storage { get; set; }
        public int? ProvisionedIops { get; set; }
        public int? StorageThroughput { get; set; }
        public Dictionary<string, string> Tags { get; set; }
    }
}
