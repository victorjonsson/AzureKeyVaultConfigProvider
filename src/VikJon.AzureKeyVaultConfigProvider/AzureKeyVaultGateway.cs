using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using System.Threading.Tasks;

namespace VikJon.AzureKeyVaultConfigProvider
{
    /**
     * This class only exists to enable mocking of the extension method IKeyVaultClient.GetSecretAsync
     */
    class KeyVaultGateway : IKeyVaultGateway
    {
        private readonly IKeyVaultClient _keyVaultClient;

        public KeyVaultGateway(IKeyVaultClient keyVaultClient)
        {
            _keyVaultClient = keyVaultClient;
        }

        public Task<SecretBundle> GetSecretAsync(string secretName, string keyVaultUrl)
        {
            return _keyVaultClient.GetSecretAsync(keyVaultUrl, secretName);
        }
    }
}
