using System;

namespace VikJon.AzureKeyVaultConfigProvider
{
    public class AzureKeyVaultException : Exception
    {
        public AzureKeyVaultException(string message) : base(message)
        {
        }
    }
}
