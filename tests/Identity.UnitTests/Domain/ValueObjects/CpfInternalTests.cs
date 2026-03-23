namespace Identity.UnitTests.Domain.ValueObjects;

public sealed class CpfInternalTests
{
    [Fact]
    public void HasValidDigits_WithInvalidLength_ShouldReturnFalse()
    {
        var method = typeof(Cpf).GetMethod(
            "HasValidDigits",
            System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)!;

        var result = (bool)method.Invoke(null, ["123"])!;

        result.Should().BeFalse();
    }
}
