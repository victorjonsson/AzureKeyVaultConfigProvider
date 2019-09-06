using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Extensions.Configuration;

namespace VikJon.AzureKeyVaultConfigProvider
{
    class KeyVaultConfigurationProvider : ConfigurationProvider
    {
        private const string AZURE_KEY_VAULT_URL_SETTING_NAME = "AZURE_KEY_VAULT_URL";

        private readonly IConfiguration _config;
        private string _azureKeyVaultUrl;
        private readonly IKeyVaultGateway _keyVaultGateway;

        public KeyVaultConfigurationProvider(IConfiguration config, string azureKeyVaultUrl, IKeyVaultGateway keyVaultGateway)
        {
            _config = config;
            _azureKeyVaultUrl = azureKeyVaultUrl;
            _keyVaultGateway = keyVaultGateway;
        }

        public override void Load()
        {
            if (string.IsNullOrEmpty(_azureKeyVaultUrl))
            {
                _azureKeyVaultUrl = _config[AZURE_KEY_VAULT_URL_SETTING_NAME]?.ToString();
            }

            var settingsWithKeyVaultRef = FilterOutSettingsWithKeyVaultRef();
            foreach(var settingName in settingsWithKeyVaultRef.Keys)
            {
                var keyVaultUrl = settingsWithKeyVaultRef[settingName].KeyVaultUrl ?? _azureKeyVaultUrl;
                var secretName = settingsWithKeyVaultRef[settingName].KeyVaultSecretName;
                if (string.IsNullOrEmpty(keyVaultUrl))
                {
                    throw new InvalidConfigException("Azure key vault url is missing when trying to fetch " + secretName);
                }

                SecretBundle secretBundle = null;
                try
                {
                    secretBundle = _keyVaultGateway.GetSecretAsync(secretName, keyVaultUrl).Result;
                }
                catch (System.Exception exception)
                {
                    throw new InvalidConfigException(
                        "Unable to fetch key vault secret for app setting '" + settingName + "', see inner exception for more info", 
                        exception
                    );
                }
                Data.Add(settingName, secretBundle == null ? "" : secretBundle.Value);
            }
        }
        
        private IDictionary<string, AzureKeyVaultReference> FilterOutSettingsWithKeyVaultRef()
        {
            var refs = new Dictionary<string, AzureKeyVaultReference>();
            FilterOutSettingsWithKeyVaultRefRecursive(_config, refs);
            return refs;
        }

        private IDictionary<string, AzureKeyVaultReference> FilterOutSettingsWithKeyVaultRefRecursive(IConfiguration configSection, IDictionary<string, AzureKeyVaultReference> refs)
        {
            foreach (var childSection in configSection.GetChildren())
            {
                FilterOutSettingsWithKeyVaultRefRecursive(childSection, refs);
                var value = childSection.Value;
                if (value != null && value.StartsWith(AzureKeyVaultReference.CONFIG_VALUE_PREFIX))
                {
                    refs.Add(childSection.Path, AzureKeyVaultReference.CreateFromString(value));
                }
            }
            return refs;
        }
    }
}
