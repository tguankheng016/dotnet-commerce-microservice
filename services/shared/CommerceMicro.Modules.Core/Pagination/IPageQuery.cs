using CommerceMicro.Modules.Core.CQRS;

namespace CommerceMicro.Modules.Core.Pagination;

public interface IPageQuery<out TResponse> : IPageRequest, IQuery<TResponse>
    where TResponse : notnull
{
}
