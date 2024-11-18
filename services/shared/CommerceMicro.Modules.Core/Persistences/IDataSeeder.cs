using CommerceMicro.Modules.Core.Dependencies;

namespace CommerceMicro.Modules.Core.Persistences;

public interface IDataSeeder : IScopedDependency
{
    Task SeedAllAsync();
}
