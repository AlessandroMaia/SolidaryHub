using Mediator.Notifications;

namespace SharedKernel.Domain;

public abstract class Entity
{
    int? _requestedHashCode;
    int _id;

    private List<INotification>? _domainEvents;
    public IReadOnlyCollection<INotification>? DomainEvents => _domainEvents?.AsReadOnly();

    public virtual int Id
    {
        get => _id;
        protected set => _id = value;
    }

    public void AddDomainEvent(INotification eventItem)
    {
        _domainEvents ??= [];
        _domainEvents.Add(eventItem);
    }

    public void RemoveDomainEvent(INotification eventItem)
        => _domainEvents?.Remove(eventItem);

    public void ClearDomainEvents()
        => _domainEvents?.Clear();

    public bool IsTransient() => Id == default;

    public override bool Equals(object? obj)
    {
        if (obj is null || obj is not Entity)
            return false;

        if (ReferenceEquals(this, obj))
            return true;

        if (GetType() != obj.GetType())
            return false;

        Entity item = (Entity)obj;

        if (item.IsTransient() || IsTransient())
            return false;

        return item.Id == Id;
    }

    public override int GetHashCode()
    {
        if (!IsTransient())
        {
            if (!_requestedHashCode.HasValue)
                _requestedHashCode = Id.GetHashCode() ^ 31;

            return _requestedHashCode.Value;
        }

        return base.GetHashCode();
    }

    public static bool operator ==(Entity left, Entity right)
    {
        if (Equals(left, null))
            return Equals(right, null);

        return left.Equals(right);
    }

    public static bool operator !=(Entity left, Entity right)
        => !(left == right);
}
