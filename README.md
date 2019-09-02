# AzureKeyVaultConfigProvider

This nuget package is an alternative to [Azure Key Vault References](https://docs.microsoft.com/en-us/azure/app-service/app-service-key-vault-references#reference-syntax) that provides 
the following benefits:

- The one major downside when it comes to key vault references is that you have to know the `secretVersion` (GUID) of the secret you're referencing. This becomes a problem
when the development teams don't have access to the key vaults in the production environment. This nuget packages **removes the need to include
the secret version** when referencing a key vault secret. Using this package makes it possible to declare key vault references looking like this: `@AzureKeyVault(mysecret, https://myvault.vault.azure.net/)`

- With this package you won't have to repeat the base url of the key vault to be used. All you have to do is to declare the configuration parameter `AZURE_KEY_VAULT_URL` once, looking something like this:

```
// appsettings.json
{
	"AZURE_KEY_VAULT_URL": "https://myvault.vault.azure.net/",
	"SENDGRID_API_KEY": "@AzureKeyVault(SendgridApiKey)",
}
```

Isn't that nice and clean looking? Having to repeat the key vault url is no major problem. But it becomes tidious to manage when having a lot of secrets and the url to the vault changes.
