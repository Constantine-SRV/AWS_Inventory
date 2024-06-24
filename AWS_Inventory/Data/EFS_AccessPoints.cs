namespace Data
{
    public class EFS_AccessPoints
    {
        public string Region { get; set; }
        public string Name { get; set; }
        public string AccessPointId { get; set; }
        public string FileSystemName { get; set; }
        public string Path { get; set; }
        public Dictionary<string, string> Tags { get; set; }
    }
}
