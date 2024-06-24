namespace Data
{
    public class ECSInstance
    {
        public string Region { get; set; }
        public string ClusterName { get; set; }
        public string ServiceName { get; set; }
        public string LoadBalancers { get; set; }
        public string TargetGroups { get; set; }
        public Dictionary<string, string> Tags { get; set; }
    }
}
