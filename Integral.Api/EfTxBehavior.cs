using MediatR;
using SharedKernel.Abstraction.CQRS;
using SharedKernel.Abstraction.Ef;

namespace Integral.Api;

public class EfTxBehavior<TRequest, TResponse>(
    UnitOfWork unitOfWork,
    EventDispatcher eventDispatcher
) : IPipelineBehavior<TRequest, TResponse> where TRequest : ICommand<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var response = await next(cancellationToken);

        await eventDispatcher.DispatchEventsAsync(cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        return response;
    }
}