namespace VerificationService.Models
{
    public class InventoryItem
    {
        public int InventoryItemId { get; set; }
        public string? Upc { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Manufacturer { get; set; }
        public int? Quantity { get; set; }
        public string? Status { get; set; }
    }
}
