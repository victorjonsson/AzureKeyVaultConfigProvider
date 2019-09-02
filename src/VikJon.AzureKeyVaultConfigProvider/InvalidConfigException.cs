namespace VikJon.AzureKeyVaultConfigProvider
{
    public class InvalidConfigException : AzureKeyVaultException
    {
        public InvalidConfigException(string message) : base(message)
        {
        }
    }
}
