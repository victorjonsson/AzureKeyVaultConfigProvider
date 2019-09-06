namespace VikJon.AzureKeyVaultConfigProvider
{
    public class ParsKeyVaultReferenceException : AzureKeyVaultException
    {
        public ParsKeyVaultReferenceException(string message) : base(message)
        {
        }
    }
}
