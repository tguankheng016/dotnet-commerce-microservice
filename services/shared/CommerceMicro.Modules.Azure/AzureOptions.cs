namespace CommerceMicro.Modules.Azure;

public class AzureOptions
{
	public bool Enabled { get; set; }

	public string KeyVaultName { get; set; } = "";

	public string TenantId { get; set; } = "";

	public string ClientId { get; set; } = "";

	public string ClientSecret { get; set; } = "";
}
