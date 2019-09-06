namespace VikJon.AzureKeyVaultConfigProvider
{
    /**
     * Value object representing a "key vault reference"
     */
    public class AzureKeyVaultReference
    {
        public const string CONFIG_VALUE_PREFIX = "@AzureKeyVault(";

        public string KeyVaultUrl { get; internal set; }
        public string KeyVaultSecretName { get; internal set; }

        public static AzureKeyVaultReference CreateFromString(string value)
        {
            var strippedValue = value.Substring(CONFIG_VALUE_PREFIX.Length);
            strippedValue = strippedValue.Substring(0, strippedValue.Length - 1);
            var chunks = strippedValue.Split(',');            
            var secretName = chunks[0].Trim();
            var vaultUrl = chunks.Length == 1 ? null : chunks[1]?.Trim();
            if (vaultUrl == "")
            {
                vaultUrl = null;
            }

            if (chunks.Length > 2)
            {
                throw new ParsKeyVaultReferenceException("Azure KeyVault reference are not allowed to contain muptiple comma signs " + value);
            }
            if (secretName == "")
            {
                throw new ParsKeyVaultReferenceException("Empty secret name not allowed");
            }

            return new AzureKeyVaultReference()
            {
                KeyVaultSecretName = secretName,
                KeyVaultUrl = vaultUrl
            };
        }

        public string ToString()
        {
            return string.Format(
                    "{0}{1}{2})",
                    CONFIG_VALUE_PREFIX,
                    KeyVaultSecretName,
                    string.IsNullOrEmpty(KeyVaultUrl) ? "" : ", " + KeyVaultUrl
                );
        }
    }
}
