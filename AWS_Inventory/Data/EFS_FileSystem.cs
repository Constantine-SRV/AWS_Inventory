namespace Data
{
    public class EFS_FileSystem
    {
        public string Region { get; set; }
        public string Name { get; set; }
        public string FileSystemId { get; set; }
        public long TotalSize { get; set; }
        public long SizeInStandard { get; set; }
        public DateTime CreationTime { get; set; }
        public Dictionary<string, string> Tags { get; set; }
    }
}
