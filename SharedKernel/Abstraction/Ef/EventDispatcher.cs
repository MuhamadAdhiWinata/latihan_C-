using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Abstraction.Domain;

namespace SharedKernel.Abstraction.Ef;

public class EventDispatcher(IEnumerable<IAppDbContext> dbContexts, IMediator mediator)
{
    private readonly IEnumerable<DbContext> _dbContexts = dbContexts.Cast<DbContext>();
    
    public async Task DispatchEventsAsync(CancellationToken cancellationToken = default)
    {
        var publishedEventIds = new HashSet<Guid>();

        while (true)
        {
            // Collect domain events from all aggregates
            var newEvents = _dbContexts
                .SelectMany(ctx => ctx.ChangeTracker.Entries())
                .Where(e => e.Entity is Aggregate)
                .Select(e => (Aggregate)e.Entity)
                .SelectMany(a => a.ClearDomainEvents())
                .Where(e => !publishedEventIds.Contains(e.EventId))
                .ToList();

            if (newEvents.Count == 0)
                break;

            foreach (var @event in newEvents)
            {
                publishedEventIds.Add(@event.EventId);
                await mediator.Publish(@event, cancellationToken);
            }
        }
    }
}

public static class EventDispatcherExtensions
{
    public static void AddEventDispatcher(this IServiceCollection services)
    {
        services.AddScoped<EventDispatcher>();
    }
}