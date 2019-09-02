using System.Threading.Tasks;
using Microsoft.Azure.KeyVault.Models;

namespace VikJon.AzureKeyVaultConfigProvider
{
    public interface IKeyVaultGateway
    {
        Task<SecretBundle> GetSecretAsync(string secretName, string keyVaultUrl);
    }
}