namespace Data
{
    public class ECSCluster
    {
        public string Region { get; set; }
        public string ClusterName { get; set; }
        public string ClusterArn { get; set; }
        public Dictionary<string, string> Tags { get; set; }
    }
}
