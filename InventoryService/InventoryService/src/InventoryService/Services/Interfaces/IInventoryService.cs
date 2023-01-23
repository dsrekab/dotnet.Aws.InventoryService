using InventoryService.Models;

namespace InventoryService.Services.Interfaces
{
    public interface IInventoryService
    {
        Task<IEnumerable<Inventory>> GetAllInventoryItems();
        Task<Inventory> GetSingleInventoryItemByUpc(int? inventoryId, string? upc);
        
        Task<Inventory> AddInventoryItem(Inventory inventoryItem);
    }
}
