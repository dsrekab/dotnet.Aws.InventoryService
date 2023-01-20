using Dapper;
using InventoryService.Models;
using InventoryService.Repositories.Interfaces;
using InventoryService.Services.Interfaces;
using MySql.Data.MySqlClient;
using System.Data;

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

        public async Task<IEnumerable<Inventory>> Get()
        {
            try
            {
                using var cxn = new MySqlConnection(_cxnString);
                return await cxn.QueryAsync<Inventory>("SELECT * FROM sys.Inventory");
            }
            catch (MySqlException ex)
            {
                _logger.LogError(ex, "Exception caught while getting items from Inventory database");
                throw;
            }
        }
    }
}
