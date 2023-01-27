using InventoryService.Models;

namespace InventoryService.Repositories.Interfaces
{
    public interface IInventoryRepository
    {
        Task<IEnumerable<Inventory>> GetAll();
        Task<Inventory> GetItemByUpc(string upc);
        Task<Inventory> GetItemByInventoryItemId(int inventoryItemId);

        Task AddItem(Inventory item);

        Task UpdateItem(Inventory item);

        Task DeleteItem(int inventoryItemId);
    }
}
