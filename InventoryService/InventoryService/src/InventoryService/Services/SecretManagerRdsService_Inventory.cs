using InventoryService.Services.Interfaces;
using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using System.Text.Json;
using InventoryService.Models;

namespace InventoryService.Services
{
    public class SecretManagerRdsService_Inventory: ISecretManagerRdsService
    {
        private RdsSecret? _cachedSecret;
        private readonly ILogger<ISecretManagerRdsService> _logger;

        public SecretManagerRdsService_Inventory(ILogger<ISecretManagerRdsService> logger)
        {
            _logger = logger;
        }

        public async Task<RdsSecret> GetSecret()
        {
            if (_cachedSecret==null)
            {
                var secretName = "inventoryService/MySql";
                var region = "us-east-1";

                var client = new AmazonSecretsManagerClient(RegionEndpoint.GetBySystemName(region));

                var request = new GetSecretValueRequest
                {
                    SecretId = secretName,
                    VersionStage = "AWSCURRENT",
                };

                GetSecretValueResponse response;

                try
                {
                    response = await client.GetSecretValueAsync(request);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Exception caught while getting secrets");
                    throw;
                }

                _cachedSecret = JsonSerializer.Deserialize<RdsSecret>(response.SecretString);
            }

            return _cachedSecret;
        }
    }
}
