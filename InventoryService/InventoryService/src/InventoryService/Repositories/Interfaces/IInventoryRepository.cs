using InventoryService.Models;

namespace InventoryService.Repositories.Interfaces
{
    public interface IInventoryRepository
    {
        Task<IEnumerable<Inventory>> Get();
    }
}
