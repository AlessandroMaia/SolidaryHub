namespace Identity.UnitTests.SharedKernel;

public sealed class CoreTests
{
    [Fact]
    public void ErrorFactories_ShouldCreateExpectedTypes()
    {
        Error.Failure("FAIL", "Falhou").Type.Should().Be(ErrorType.Failure);
        Error.NotFound("NOT_FOUND", "Nao encontrado").Type.Should().Be(ErrorType.NotFound);
        Error.Problem("PROBLEM", "Problema").Type.Should().Be(ErrorType.Problem);
        Error.Conflict("CONFLICT", "Conflito").Type.Should().Be(ErrorType.Conflict);
    }

    [Fact]
    public void PagedResponse_ShouldCalculateTotalPages()
    {
        var response = new PagedResponse<int>([1, 2], 2, 2, 5);

        response.PageNumber.Should().Be(2);
        response.PageSize.Should().Be(2);
        response.TotalRecords.Should().Be(5);
        response.TotalPages.Should().Be(3);
        response.Data.Should().Equal(1, 2);
    }

    [Fact]
    public void EntityEquality_ShouldRespectIdentity()
    {
        var left = new TestEntity();
        var right = new TestEntity();
        var other = new OtherTestEntity();

        left.IsTransient().Should().BeTrue();
        (left == right).Should().BeFalse();

        SetId(left, 1);
        SetId(right, 1);
        SetId(other, 1);

        left.Should().Be(right);
        left.GetHashCode().Should().Be(right.GetHashCode());
        left.Equals(other).Should().BeFalse();
        (left != other).Should().BeTrue();
        new TestEntity().GetHashCode().Should().NotBe(0);
    }

    [Fact]
    public void ValueObject_ShouldSupportEqualityAndCopy()
    {
        var left = new TestValueObject("A", 1);
        var right = new TestValueObject("A", 1);
        var other = new TestValueObject("B", 2);

        left.Should().Be(right);
        (left == right).Should().BeTrue();
        (left != other).Should().BeTrue();
        left.GetCopy().Should().BeEquivalentTo(left);
    }

    [Fact]
    public void Constants_ShouldExposeExpectedValues()
    {
        Roles.Manager.Should().Be("NGO_MANAGER");
        Roles.Donor.Should().Be("DONOR");
        AuthorizationPolicies.RequireManagerUser.Should().Be("RequireManagerUser");
        TokenSettings.SectionName.Should().Be("JwtSettings");
    }

    private static void SetId(Entity entity, int id)
    {
        typeof(Entity).GetProperty(nameof(Entity.Id))!.SetValue(entity, id);
    }

    private sealed class TestEntity : Entity;
    private sealed class OtherTestEntity : Entity;

    private sealed class TestValueObject(string text, int number) : ValueObject
    {
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return text;
            yield return number;
        }
    }
}
