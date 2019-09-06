using System;

namespace VikJon.AzureKeyVaultConfigProvider
{
    public class AzureKeyVaultException : Exception
    {
        public AzureKeyVaultException(string message) : base(message)
        {
        }

        public AzureKeyVaultException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
