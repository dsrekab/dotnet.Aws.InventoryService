using VerificationService.Models;
using VerificationService.Services;
using VerificationService.Services.Interfaces;

namespace VerificationService
{
    public static class StartupExtension
    {
        public static void AddVerificationServices(this IServiceCollection services)
        {
            services.AddSingleton<IVerificationService<InventoryItem>, InventoryItemVerificationService>();
        }
    }
}
