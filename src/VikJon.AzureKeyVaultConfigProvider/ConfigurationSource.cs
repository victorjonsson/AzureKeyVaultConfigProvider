using Microsoft.Extensions.Configuration;

namespace VikJon.AzureKeyVaultConfigProvider
{
    class ConfigurationSource : IConfigurationSource
    {
        private readonly IConfiguration _config;
        private readonly string _azureKeyVaultUrl;
        private readonly IKeyVaultGateway _keyVaultGateway;

        public ConfigurationSource(IConfiguration conf, string azureKeyVaultUrl, IKeyVaultGateway keyVaultGateway)
        {
            _config = conf;
            _azureKeyVaultUrl = azureKeyVaultUrl;
            _keyVaultGateway = keyVaultGateway;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new KeyVaultConfigurationProvider(_config, _azureKeyVaultUrl, _keyVaultGateway);
        }
    }
}
