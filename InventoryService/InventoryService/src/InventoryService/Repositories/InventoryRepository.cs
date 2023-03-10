using Dapper;
using InventoryService.Models;
using InventoryService.Repositories.Interfaces;
using InventoryService.Services.Interfaces;
using MySql.Data.MySqlClient;

namespace InventoryService.Repositories
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly string _cxnString;
        private readonly ILogger<IInventoryRepository> _logger;

        public InventoryRepository(ISecretManagerRdsService secretsManagerRdsService, ILogger<IInventoryRepository> logger)
        {
            var rdsSecret = secretsManagerRdsService.GetSecret().Result;
            _cxnString = $"server={rdsSecret.host};uid={rdsSecret.username};pwd={rdsSecret.password};database=sys";

            _logger = logger;
        }

        public async Task<IEnumerable<Inventory>> GetAll()
        {
            try
            {
                using var cxn = new MySqlConnection(_cxnString);
                return await cxn.QueryAsync<Inventory>("SELECT * FROM inventory.InventoryItems");
            }
            catch (MySqlException ex)
            {
                _logger.LogError(ex, "Exception caught while getting all items from Inventory database");
                throw;
            }
        }
        public async Task<Inventory> GetItemByUpc(string upc)
        {
            try
            {
                using var cxn = new MySqlConnection(_cxnString);
                return await cxn.QuerySingleOrDefaultAsync<Inventory>("SELECT * FROM inventory.InventoryItems where Upc = @upc",
                    new { upc = upc });
            }
            catch (MySqlException ex)
            {
                _logger.LogError(ex, "Exception caught while getting item by UPC from Inventory database");
                throw;
            }
        }

        public async Task<Inventory> GetItemByInventoryItemId(int inventoryItemId)
        {
            try
            {
                using var cxn = new MySqlConnection(_cxnString);
                return await cxn.QuerySingleOrDefaultAsync<Inventory>("SELECT * FROM inventory.InventoryItems where InventoryItemId = @id",
                    new {id = inventoryItemId});
            }
            catch (MySqlException ex)
            {
                _logger.LogError(ex, "Exception caught while getting item by InventoryID from Inventory database");
                throw;
            }
        }

        public async Task AddItem(Inventory inventoryItem)
        {
            try
            {
                using var cxn = new MySqlConnection(_cxnString);
                await cxn.ExecuteAsync("INSERT INTO inventory.InventoryItems (Upc, Name, Description, Manufacturer, Quantity, Status) VALUES (@upc, @name, @description, @manufacturer, @quantity, @status)",
                    new { 
                        upc = inventoryItem.Upc,
                        name=inventoryItem.Name,
                        description = inventoryItem.Description,
                        manufacturer = inventoryItem.Manufacturer,
                        quantity = inventoryItem.Quantity,
                        status = "Active"
                    });
            }
            catch (MySqlException ex)
            {
                _logger.LogError(ex, $"Exception caught while executing AddItem for UPC {inventoryItem.Upc } into Inventory database");
                throw;
            }
        }

        public async Task UpdateItem(Inventory inventoryItem)
        {
            try
            {
                using var cxn = new MySqlConnection(_cxnString);
                await cxn.ExecuteAsync("UPDATE inventory.InventoryItems SET Upc=@upc, Name=@name, Description=@description, Manufacturer=@manufacturer, Quantity=@quantity, status=@status WHERE InventoryItemId = @inventoryId",
                    new
                    {
                        inventoryId = inventoryItem.InventoryItemId,
                        upc = inventoryItem.Upc,
                        name = inventoryItem.Name,
                        description = inventoryItem.Description,
                        manufacturer = inventoryItem.Manufacturer,
                        quantity = inventoryItem.Quantity,
                        status = inventoryItem.Status
                    });
            }
            catch (MySqlException ex)
            {
                _logger.LogError(ex, $"Exception caught while executing Update for UPC {inventoryItem.Upc} into Inventory database");
                throw;
            }
        }

        public async Task DeleteItem(int inventoryItemId)
        {
            try
            {
                using var cxn = new MySqlConnection(_cxnString);
                await cxn.ExecuteAsync("DELETE FROM inventory.InventoryItems WHERE InventoryItemId = @inventoryId",
                    new
                    {
                        inventoryId = inventoryItemId
                    });
            }
            catch (MySqlException ex)
            {
                _logger.LogError(ex, $"Exception caught while executing Delete for InventoryItemId {inventoryItemId} from Inventory database");
                throw;
            }
        }
    }
}
