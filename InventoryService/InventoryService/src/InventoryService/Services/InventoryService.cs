using InventoryService.Exceptions;
using InventoryService.Models;
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

        public async Task<Inventory> AddInventoryItem(Inventory inventoryItem)
        {
            await VerifyInventoryItemForAdd(inventoryItem);

            await _inventoryRepository.AddItem(inventoryItem);

            return await _inventoryRepository.GetItemByUpc(inventoryItem.Upc);
        }

        public async Task DeleteInventoryItem(Inventory inventoryItem)
            => await _inventoryRepository.DeleteItem(inventoryItem.InventoryId);

        public Task<IEnumerable<Inventory>> GetAllInventoryItems()
            => _inventoryRepository.GetAll();

        public Task<Inventory> GetSingleInventoryItem(int? inventoryId, string? upc)
        {
            if (inventoryId != null)
            {
                return _inventoryRepository.GetItemByInventoryId(inventoryId.Value);
            }

            if (upc != null)
            {
                return _inventoryRepository.GetItemByUpc(upc);
            }

            throw new InventoryServiceException("You must supply an inventoryId or a Upc to select a single InventoryItem.");
        }

        public async Task<Inventory> UpdateInventoryItem(Inventory inventoryItem)
        {
            await VerifyInventoryItemForUpdate(inventoryItem);

            await _inventoryRepository.UpdateItem(inventoryItem);

            return await _inventoryRepository.GetItemByUpc(inventoryItem.Upc);
        }

        private async Task VerifyInventoryItemForAdd(Inventory inventoryItem)
        {
            if (string.IsNullOrWhiteSpace(inventoryItem.Upc))
            {
                throw new InventoryServiceException("InventoryItem parameter is invalid for AddItem command.");
            }

            var existing = await _inventoryRepository.GetItemByUpc(inventoryItem.Upc);

            if (existing != null)
            {
                throw new InventoryServiceException($"{inventoryItem.Upc} already exists in the Inventory Database.");
            }
        }

        private async Task VerifyInventoryItemForUpdate(Inventory inventoryItem)
        {
            if (inventoryItem == null || string.IsNullOrEmpty(inventoryItem.Upc))
            {
                throw new InventoryServiceException("You must provide an Inventory Item to Update");
            }

            var existing = await _inventoryRepository.GetItemByUpc(inventoryItem.Upc);

            if (existing==null)
            {
                throw new InventoryServiceException($"Upc {inventoryItem.Upc} does not exist in the inventory database.");
            }
        }
    }
}
