using System;

namespace VikJon.AzureKeyVaultConfigProvider
{
    public class InvalidConfigException : AzureKeyVaultException
    {
        public InvalidConfigException(string message) : base(message)
        {
        }

        public InvalidConfigException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
