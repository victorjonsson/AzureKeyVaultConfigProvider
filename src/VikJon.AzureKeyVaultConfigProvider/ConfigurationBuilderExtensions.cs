using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;

namespace VikJon.AzureKeyVaultConfigProvider
{
    public static class ConfigurationBuilderExtensions
    {
        public static IConfigurationBuilder AddAzureKeyVaultWithNameRefSupport(
            this IConfigurationBuilder builder, string azureKeyVaultUrl = null, IKeyVaultGateway keyVaultGateway = null)
        {
            if (keyVaultGateway == null)
            {
                var azureServiceTokenProvider = new AzureServiceTokenProvider();
                var keyVaultAuthCallback = new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback);
                var keyVaultClient = new KeyVaultClient(keyVaultAuthCallback);
                keyVaultGateway = new AzureKeyVaultGateway(keyVaultClient);
            }
            return builder.Add(new ConfigurationSource(builder.Build(), azureKeyVaultUrl, keyVaultGateway));
        }

    }
}
