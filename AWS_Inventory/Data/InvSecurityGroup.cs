namespace AWS_Inventory.Data
{
    public class InvSecurityGroup
    {
        public string Region { get; set; }
        public string Name { get; set; }
        public string GroupId { get; set; }
        public string Description { get; set; }
        public string VpcId { get; set; }
        public string Owner { get; set; }
        public Dictionary<string, string> Tags { get; set; }
    }
}
