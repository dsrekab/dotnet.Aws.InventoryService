using InventoryService.Models;

namespace InventoryService.Repositories.Interfaces
{
    public interface IInventoryRepository
    {
        Task<IEnumerable<Inventory>> GetAll();
        Task<Inventory> GetItemByUpc(string upc);
        Task<Inventory> GetItemByInventoryId(int inventoryItemId);
        Task<Inventory> AddItem(Inventory item);
    }
}
