namespace AWS_Inventory.Data
{
    public class AWSSecret
    {
        public string SecretName { get; set; }
        public DateTime? LastRetrieved { get; set; }
        public Dictionary<string, string> Tags { get; set; }
    }
}
