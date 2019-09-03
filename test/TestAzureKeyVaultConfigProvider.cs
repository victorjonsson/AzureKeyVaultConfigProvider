using Microsoft.Azure.KeyVault.Models;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace VikJon.AzureKeyVaultConfigProvider
{
    public class TestAzureKeyVaultConfigProvider
    {
        private const string NameOfConfigHavingSecret = "ConfigWithSecret";
        private const string SecretName = "SecretName";
        private const string SecretValue = "SomeSuperSecretValue";
        private const string KeyVaultUrl = "https://azurekeyvault.com";

        [Fact]
        public void test_that_we_dont_crasch_when_feature_isnt_used()
        {
            var configBuilder = new ConfigurationBuilder();
            configBuilder.AddInMemoryCollection(new Dictionary<string, string>() {
                        {"Something", "HasValue"}
                    });
            configBuilder.AddAzureKeyVaultWithNameRefSupport();
            configBuilder.Build();
        }

        [Fact]
        public void test_that_we_get_expected_exception_when_key_vault_url_is_missing()
        {
            var configBuilder = new ConfigurationBuilder();
            configBuilder.AddInMemoryCollection(new Dictionary<string, string>() {
                        {NameOfConfigHavingSecret, GetKeyVaultRefMissingVaultUrl()}
                    });
            configBuilder.AddAzureKeyVaultWithNameRefSupport();
            Assert.Throws<InvalidConfigException>(() => configBuilder.Build());
        }

        [Fact]
        public void test_that_we_can_fetch_secret_from_key_vault()
        {
            var keyVaultGateway = new Mock<IKeyVaultGateway>();
            var secretBundle = new SecretBundle() { Value = SecretValue };
            keyVaultGateway
                .Setup(m => m.GetSecretAsync(SecretName, KeyVaultUrl))
                .Returns(Task.FromResult(secretBundle));

            var configBuilder = new ConfigurationBuilder();
            configBuilder.AddInMemoryCollection(GetConfig(GetKeyVaultRef()));
            configBuilder.AddAzureKeyVaultWithNameRefSupport(null, keyVaultGateway.Object);
            var config = configBuilder.Build();
            Assert.Equal(SecretValue, config[NameOfConfigHavingSecret]?.ToString());
        }

        [Fact]
        public void test_that_we_can_fetch_secret_having_keyvault_url_present_in_config()
        {
            var keyVaultGateway = new Mock<IKeyVaultGateway>();
            var secretBundle = new SecretBundle() { Value = SecretValue };
            keyVaultGateway
                .Setup(m => m.GetSecretAsync(SecretName, "https://somekeyvaulthere.com"))
                .Returns(Task.FromResult(secretBundle));

            var config = GetConfig(GetKeyVaultRefMissingVaultUrl());
            config.Add("AZURE_KEY_VAULT_URL", "https://somekeyvaulthere.com");
            var configBuilder = new ConfigurationBuilder();
            configBuilder.AddInMemoryCollection(config);
            configBuilder.AddAzureKeyVaultWithNameRefSupport(null, keyVaultGateway.Object);
            var newConfig = configBuilder.Build();
            Assert.Equal(SecretValue, newConfig[NameOfConfigHavingSecret]?.ToString());
        }

        [Fact]
        public void test_that_we_can_fetch_secret_having_keyvault_url_provided_through_code()
        {
            var keyVaultUrl = "https://somekeyvaulthere.com";
            var keyVaultGateway = new Mock<IKeyVaultGateway>();
            var secretBundle = new SecretBundle() { Value = SecretValue };
            keyVaultGateway
                .Setup(m => m.GetSecretAsync(SecretName, "https://somekeyvaulthere.com"))
                .Returns(Task.FromResult(secretBundle));

            var config = GetConfig(GetKeyVaultRefMissingVaultUrl());
            var configBuilder = new ConfigurationBuilder();
            configBuilder.AddInMemoryCollection(config);
            configBuilder.AddAzureKeyVaultWithNameRefSupport(keyVaultUrl, keyVaultGateway.Object);
            var newConfig = configBuilder.Build();
            Assert.Equal(SecretValue, newConfig[NameOfConfigHavingSecret]?.ToString());
        }

        [Fact]
        public void test_that_we_can_fetch_multiple_secrets()
        {
            var keyVaultGateway = new Mock<IKeyVaultGateway>();
            keyVaultGateway
                .Setup(m => m.GetSecretAsync(SecretName, KeyVaultUrl))
                .Returns(Task.FromResult(new SecretBundle() { Value = SecretValue }));
            keyVaultGateway
                .Setup(m => m.GetSecretAsync("anotherSecret", KeyVaultUrl))
                .Returns(Task.FromResult(new SecretBundle() { Value = "anotherSecretValue" }));

            var config = GetConfig(GetKeyVaultRef());
            config.Add("AnotherConfigWithSecret", GetKeyVaultRef(nameOfSecret: "anotherSecret"));
            var configBuilder = new ConfigurationBuilder();
            configBuilder.AddInMemoryCollection(config);
            configBuilder.AddAzureKeyVaultWithNameRefSupport(null, keyVaultGateway.Object);
            var newConfig = configBuilder.Build();
            Assert.Equal(SecretValue, newConfig[NameOfConfigHavingSecret]?.ToString());
            Assert.Equal("anotherSecretValue", newConfig["AnotherConfigWithSecret"]?.ToString());
        }

        [Fact]
        public void test_that_we_can_fetch_secrets_when_having_complex_objects_in_config()
        {
            var keyVaultGateway = new Mock<IKeyVaultGateway>();
            var secretBundle = new SecretBundle() { Value = SecretValue };
            keyVaultGateway
                .Setup(m => m.GetSecretAsync("SecretFromJsonFile", "https://keyvaultdeclaredinappjson.com"))
                .Returns(Task.FromResult(secretBundle));

            var configBuilder = new ConfigurationBuilder();
            configBuilder.AddJsonFile($"app-settings.json");
            configBuilder.AddAzureKeyVaultWithNameRefSupport(null, keyVaultGateway.Object);
            var newConfig = configBuilder.Build();
            var complexConfigObj = newConfig.GetSection("ComplexObject");
            Assert.Equal(SecretValue, complexConfigObj["SettingWithSecretInAppSettingsJson"]?.ToString());
            Assert.Equal("hasvalue", complexConfigObj["something"]?.ToString());
            Assert.Equal("stillhasvalue", newConfig["rootLevel"]?.ToString());
        }

        private static IDictionary<string, string> GetConfig(string keyVaultRef)
        {
            return new Dictionary<string, string>() {
                {"Something", "HasValue"},
                {"Something:else", "HasOtherValue"},
                {NameOfConfigHavingSecret, keyVaultRef}
            };
        }

        private static string GetKeyVaultRefMissingVaultUrl()
        {
            return "@AzureKeyVault(" + SecretName + ")";
        }

        private static string GetKeyVaultRef(string nameOfSecret = SecretName, string keyVaultUrl = KeyVaultUrl)
        {
            return "@AzureKeyVault(" + nameOfSecret + ", " + keyVaultUrl + ")";
        }
    }
}
