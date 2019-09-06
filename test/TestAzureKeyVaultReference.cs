using Xunit;

namespace VikJon.AzureKeyVaultConfigProvider
{
    public class TestAzureKeyVaultReference
    {
        [Fact]
        public void test_that_we_can_parse_key_vault_references()
        {
            var refString = "@AzureKeyVault(secretName, vaultUrl)";
            var keyVaultRef = AzureKeyVaultReference.CreateFromString(refString);
            Assert.Equal("secretName", keyVaultRef.KeyVaultSecretName);
            Assert.Equal("vaultUrl", keyVaultRef.KeyVaultUrl);
            Assert.Equal(refString, keyVaultRef.ToString());

            // assert trimming of value and url
            refString = "@AzureKeyVault(   secretName  , vaultUrl  )";
            keyVaultRef = AzureKeyVaultReference.CreateFromString(refString);
            Assert.Equal("secretName", keyVaultRef.KeyVaultSecretName);
            Assert.Equal("vaultUrl", keyVaultRef.KeyVaultUrl);
            Assert.Equal("@AzureKeyVault(secretName, vaultUrl)", keyVaultRef.ToString());

            // assert that we can ommitt url
            refString = "@AzureKeyVault(secretName)";
            keyVaultRef = AzureKeyVaultReference.CreateFromString(refString);
            Assert.Equal("secretName", keyVaultRef.KeyVaultSecretName);
            Assert.Null(keyVaultRef.KeyVaultUrl);
            Assert.Equal(refString, keyVaultRef.ToString());

            // assert that we can handle sloppy config managers
            refString = "@AzureKeyVault(secretName, )";
            keyVaultRef = AzureKeyVaultReference.CreateFromString(refString);
            Assert.Equal("secretName", keyVaultRef.KeyVaultSecretName);
            Assert.Null(keyVaultRef.KeyVaultUrl);
            Assert.Equal("@AzureKeyVault(secretName)", keyVaultRef.ToString());
        }

        [Fact]
        public void test_that_we_get_expected_exception_when_having_invalid_format()
        {
            Assert.Throws<ParsKeyVaultReferenceException>(() => {
                AzureKeyVaultReference.CreateFromString("@AzureKeyVault()");
            });

            Assert.Throws<ParsKeyVaultReferenceException>(() => {
                AzureKeyVaultReference.CreateFromString("@AzureKeyVault(a,b,c)");
            });
        }
    }
}
