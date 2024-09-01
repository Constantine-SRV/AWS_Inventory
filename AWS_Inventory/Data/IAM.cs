namespace AWS_Inventory.Data
{
    public class InvIamUser
    {
        public string Region { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string Arn { get; set; }
        public string Path { get; set; }
        public string PasswordLastUsed { get; set; }
        public Dictionary<string, string> Tags { get; set; }
    }

    public class InvIamRole
    {
        public string Region { get; set; }
        public string RoleName { get; set; }
        public string RoleId { get; set; }
        public string Arn { get; set; }
        public string Path { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Dictionary<string, string> Tags { get; set; }
    }

    public class InvIamPolicy
    {
        public string Region { get; set; }
        public string PolicyName { get; set; }
        public string PolicyId { get; set; }
        public string Arn { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool IsAttachable { get; set; }
        public string DefaultVersionId { get; set; }
        public Dictionary<string, string> Tags { get; set; }
    }
}
