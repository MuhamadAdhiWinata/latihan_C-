using SharedKernel.Abstraction.Domain;

namespace Integral.Api.Features.Master.Items.Events;

public record ItemCreated(string Code) : IDomainEvent
{
}