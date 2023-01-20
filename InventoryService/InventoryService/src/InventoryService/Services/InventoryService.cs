using InventoryService.Repositories.Interfaces;
using InventoryService.Services.Interfaces;

namespace InventoryService.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepository;

        public InventoryService(IInventoryRepository inventoryRepository)
        {
            _inventoryRepository = inventoryRepository;
        }

        public async Task GetInventoryItem()
        {
           var p = await _inventoryRepository.Get();
        }
    }
}
