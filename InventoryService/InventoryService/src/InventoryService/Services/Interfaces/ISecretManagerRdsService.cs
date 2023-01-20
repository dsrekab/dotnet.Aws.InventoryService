using InventoryService.Models;

namespace InventoryService.Services.Interfaces
{
    public interface ISecretManagerRdsService
    {
        Task<RdsSecret> GetSecret();
    }
}
