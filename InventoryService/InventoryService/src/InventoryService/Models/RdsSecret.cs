namespace InventoryService.Models
{
    public class RdsSecret
    {
        public string username { get; set; }
        public string password { get; set; }
        public string engine { get; set; }
        public string host { get; set; }
        public int port { get; set; }
        public string dbInstanceIdentifier { get; set; }
    }
}
