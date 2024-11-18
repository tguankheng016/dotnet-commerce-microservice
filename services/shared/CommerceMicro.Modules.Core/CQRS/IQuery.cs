using MediatR;

namespace CommerceMicro.Modules.Core.CQRS;

public interface IQuery<out T> : IRequest<T>
    where T : notnull
{
}