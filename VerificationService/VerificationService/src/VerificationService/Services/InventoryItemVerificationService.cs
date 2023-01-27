using VerificationService.Models;
using VerificationService.Services.Interfaces;

namespace VerificationService.Services
{
    public class InventoryItemVerificationService : IVerificationService<InventoryItem>
    {
        public Task<bool> VerifyAllRequiredFields(InventoryItem itemToVerify)
        {
            return Task.FromResult(!string.IsNullOrWhiteSpace(itemToVerify.Upc) && !string.IsNullOrWhiteSpace(itemToVerify.Name));
        }
    }
}
