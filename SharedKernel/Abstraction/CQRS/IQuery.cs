using MediatR;

namespace SharedKernel.Abstraction.CQRS;

public interface IQuery<out T> : IRequest<T>
    where T : notnull
{
}