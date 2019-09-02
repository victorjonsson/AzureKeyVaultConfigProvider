namespace VikJon.AzureKeyVaultConfigProvider
{
    /**
     * Value object representing a "key vault reference"
     */
    internal class AzureKeyVaultReference
    {
        public const string CONFIG_VALUE_PREFIX = "@AzureKeyVault(";

        public string KeyVaultUrl { get; internal set; }
        public string KeyVaultSecretName { get; internal set; }

        public static AzureKeyVaultReference CreateFromString(string value)
        {
            var strippedValue = value.Substring(CONFIG_VALUE_PREFIX.Length);
            strippedValue = strippedValue.Substring(0, strippedValue.Length - 1);
            var strippedValueChunks = strippedValue.Split(",");
            if (strippedValueChunks.Length > 2)
            {
                throw new InvalidConfigException("Azure KeyVault reference are not allowed to contain muptiple comma signs " + value);
            }

            return new AzureKeyVaultReference()
            {
                KeyVaultSecretName = strippedValueChunks[0].Trim(),
                KeyVaultUrl = strippedValueChunks.Length == 1 ? null : strippedValueChunks[1]?.Trim(),
            };
        }
    }
}
