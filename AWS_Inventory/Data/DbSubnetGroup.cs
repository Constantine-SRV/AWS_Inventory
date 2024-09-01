namespace AWS_Inventory.Data
{
    public class DbSubnetGroup
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> SubnetIds { get; set; }
        public Dictionary<string, string> Tags { get; set; }
    }
}
