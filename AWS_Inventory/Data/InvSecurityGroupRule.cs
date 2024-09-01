namespace AWS_Inventory.Data
{
    public class InvSecurityGroupRule
    {
        public string SecurityGroupId { get; set; } // ID группы безопасности
        public string Direction { get; set; } // "Inbound" или "Outbound"
        public string SourceOrDestination { get; set; } // Source для Inbound или Destination для Outbound
        public string Protocol { get; set; }
        public int FromPort { get; set; }
        public int ToPort { get; set; }
    }
}
