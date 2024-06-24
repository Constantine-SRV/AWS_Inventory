namespace Data
{
    public class S3
    {
        public string Region { get; set; }
        public string Name { get; set; }
        public DateTime CreationDate { get; set; }
        public Dictionary<string, string> Tags { get; set; }
    }
}
