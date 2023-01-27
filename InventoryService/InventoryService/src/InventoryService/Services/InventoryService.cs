using InventoryService.Exceptions;
using InventoryService.Models;
using InventoryService.Repositories.Interfaces;
using InventoryService.Services.Interfaces;

namespace InventoryService.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly ILogger<IInventoryService> _logger;

        public InventoryService(IInventoryRepository inventoryRepository, ILogger<IInventoryService> logger)
        {
            _inventoryRepository = inventoryRepository;
            _logger = logger;
        }

        public async Task<Inventory> AddInventoryItem(Inventory inventoryItem)
        {
            _logger.LogInformation("Verifying Inventory Item prior to adding Upc {Upc}...", inventoryItem.Upc);
            await VerifyInventoryItemForAdd(inventoryItem);

            _logger.LogInformation("Adding Inventory Item for Upc {Upc}...", inventoryItem.Upc);
            await _inventoryRepository.AddItem(inventoryItem);

            _logger.LogInformation("Inventory Item for Upc {Upc} added to database...", inventoryItem.Upc);
            return await _inventoryRepository.GetItemByUpc(inventoryItem.Upc);
        }

        public async Task DeleteInventoryItem(int inventoryId)
        {
            _logger.LogInformation("Deleting Inventory Item {InventoryItemId} from database...", inventoryId);

            await _inventoryRepository.DeleteItem(inventoryId);
        }

        public Task<IEnumerable<Inventory>> GetAllInventoryItems()
        {
            _logger.LogInformation("Getting all InventoryItems from database...");

            var retVal = _inventoryRepository.GetAll();

            return retVal;
        }


        public Task<Inventory> GetSingleInventoryItem(int? inventoryId, string? upc)
        {
            if (inventoryId != null)
            {
                _logger.LogInformation("Getting Inventory Item for InventoryItemId {InventoryId}...", inventoryId);
                return _inventoryRepository.GetItemByInventoryItemId(inventoryId.Value);
            }

            if (upc != null)
            {
                _logger.LogInformation("Getting Inventory Item for Upc {Upc}...", upc);
                return _inventoryRepository.GetItemByUpc(upc);
            }

            throw new InventoryServiceException("You must supply an inventoryItemId or a Upc to select a single InventoryItem.");
        }

        public async Task<Inventory> UpdateInventoryItem(Inventory inventoryItem)
        {
            _logger.LogInformation("Verifying Inventory Item prior to updating Upc {Upc}...", inventoryItem.Upc);
            await VerifyInventoryItemForUpdate(inventoryItem);

            _logger.LogInformation("Updating Inventory Item for Upc {Upc}...", inventoryItem.Upc);
            await _inventoryRepository.UpdateItem(inventoryItem);

            _logger.LogInformation("Inventory Item for Upc {Upc} updated in database...", inventoryItem.Upc);
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
