namespace SharedKernel;

public abstract class ValueObject
{
    protected abstract IEnumerable<object> GetEqualityComponents();

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (obj.GetType() != GetType()) return false;

        var other = (ValueObject)obj;

        return this.GetEqualityComponents()
            .SequenceEqual(other.GetEqualityComponents());
    }

    public override int GetHashCode()
        => GetEqualityComponents()
            .Select(x => x != null ? x.GetHashCode() : 0)
            .Aggregate((x, y) => x ^ y);

    public ValueObject? GetCopy() => MemberwiseClone() as ValueObject;

    public static bool operator ==(ValueObject left, ValueObject right)
    {
        if (left is null ^ right is null) return false;
        if (left is null && right is null) return true;
        if (left is null || right is null) return false;

        return left.Equals(right);
    }

    public static bool operator !=(ValueObject left, ValueObject right)
        => !(left == right);
}
