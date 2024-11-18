namespace CommerceMicro.Modules.Postgres;

public class PostgresOptions
{
	public string ConnectionString { get; set; }

	public PostgresOptions()
	{
		ConnectionString = "";
	}
}
