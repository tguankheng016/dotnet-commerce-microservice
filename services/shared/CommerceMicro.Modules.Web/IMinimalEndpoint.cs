using Microsoft.AspNetCore.Routing;

namespace CommerceMicro.Modules.Web;

public interface IMinimalEndpoint
{
    IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder);
}
