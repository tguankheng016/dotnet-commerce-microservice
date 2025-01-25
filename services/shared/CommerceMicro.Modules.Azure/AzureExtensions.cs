using Microsoft.Extensions.DependencyInjection;
using CommerceMicro.Modules.Core.Configurations;
using Azure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace CommerceMicro.Modules.Azure;

public static class AzureExtensions
{
	public static WebApplicationBuilder LoadAzureKeyVault(this WebApplicationBuilder builder)
	{
		builder.Services.AddValidateOptions<AzureOptions>();
		var azureOptions = builder.Services.BuildServiceProvider().GetRequiredService<AzureOptions>();

		if (azureOptions.Enabled)
		{
			var azureKeyVaultUrl = $"https://{azureOptions.KeyVaultName}.vault.azure.net/";

			builder.Configuration.AddAzureKeyVault(
				new Uri(azureKeyVaultUrl),
				new ClientSecretCredential(
					azureOptions.TenantId,
					azureOptions.ClientId,
					azureOptions.ClientSecret
				)
			);
		}

		return builder;
	}
}
