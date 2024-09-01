namespace AWS_Inventory.Data
{
    public class InvLaunchTemplate
    {
        public string Region { get; set; } 
        public string TemplateId { get; set; }
        public string TemplateName { get; set; }
        public string DefaultVersion { get; set; }
        public string LatestVersion { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Dictionary<string, string> Tags { get; set; }
    }
}
