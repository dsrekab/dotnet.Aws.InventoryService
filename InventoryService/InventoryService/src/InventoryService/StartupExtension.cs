using InventoryService.Services;
using InventoryService.Services.Interfaces;

namespace InventoryService
{
    public static class StartupExtension
    {
        public static void AddInventoryServices(this IServiceCollection services)
        {
            services.AddTransient<IInventoryService, Services.InventoryService>();

            services.AddSingleton<ISecretsManagerService, SecretsManagerService>();
        }
    }
}
