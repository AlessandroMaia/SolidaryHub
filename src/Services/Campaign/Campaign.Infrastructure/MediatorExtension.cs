namespace Campaign.Infrastructure;

static class MediatorExtension
{
    public static async Task DispatchDomainEventsAsync(this IMediator mediator, CampaignContext ctx)
    {
        var domainEntities = ctx.ChangeTracker
            .Entries<Entity>()
            .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Count > 0);

        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.DomainEvents ?? Enumerable.Empty<INotification>())
            .ToList();

        domainEntities.ToList()
            .ForEach(entity => entity.Entity.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
            await mediator.Publish(domainEvent);
    }
}
