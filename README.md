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

Isn't that nice and clean looking? Having to repeat the key vault url is of course no major problem. But it becomes tidious to manage when having a lot of secrets and the url to the vault changes.

## Setup

After that you've installed the package using nuget you have to invoke the extension method `VikJon.AzureKeyVaultConfigProvider.AddAzureKeyVaultWithNameRefSupport` on the ConfigurationBuilder of your .netcore app. This usually looks something like this in `Program.cs`

```C#
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using VikJon.AzureKeyVaultConfigProvider;

public class Program
{
    public static void Main(string[] args)
    {
        CreateWebHostBuilder(args).Build().Run();
    }

    public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
            	config
                  .SetBasePath(Directory.GetCurrentDirectory())
                  .AddJsonFile($"appsettings.json", optional: true, reloadOnChange: true)
                  .AddEnvironmentVariables()
                  .AddCommandLine(args)
		  .AddAzureKeyVaultWithNameRefSupport()
            })
            .UseStartup<Startup>();
}
```
**Notice!** You should not invoke `config.AddAzureKeyVault` on the ConfigurationBuilder

Now you can start referencing key vault secrets using the following syntax `@AzureKeyVault([SECRET_NAME], [KEY_VAULT_URL:optional])` 

```
// appsettings.json
{
	"AZURE_KEY_VAULT_URL": "https://myvault.vault.azure.net/",
	"SENDGRID_API_KEY": "@AzureKeyVault(SendgridApiKey)"
}
```

You have the option to provide the base url of your key vault instance on every call to @AzureKeyVault(...) or you can add a single configuration parameter named `AZURE_KEY_VAULT_URL`, containing the base url.

## Authentication

This package takes for granted that you're using [Managed Identites](https://docs.microsoft.com/en-us/aspnet/core/security/key-vault-configuration?view=aspnetcore-2.2#use-managed-identities-for-azure-resources) to authenticate against the key vault. If that is not the case you need to implement [VikJon.AzureKeyVaultConfigProvider.IKeyVaultGateway](https://github.com/victorjonsson/dotnet-AzureKeyVaultConfigProvider/blob/master/src/VikJon.AzureKeyVaultConfigProvider/IKeyVaultGateway.cs) and provide the extension method `AddAzureKeyVaultWithNameRefSupport` with an instance of that implementation. 

