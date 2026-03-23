namespace Identity.UnitTests.SharedKernel;

public sealed class ResultTests
{
    [Fact]
    public void Success_ShouldCreateSuccessfulResult()
    {
        var result = Result.Success();

        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Error.Should().Be(Error.None);
    }

    [Fact]
    public void Failure_ShouldCreateFailureResult()
    {
        var error = Error.Failure("IDENTITY_ERROR", "Erro");

        var result = Result.Failure(error);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void GenericSuccess_ShouldExposeValue()
    {
        var result = Result.Success(10);

        result.Value.Should().Be(10);
    }

    [Fact]
    public void GenericFailure_ValueAccessShouldThrow()
    {
        var result = Result.Failure<int>(Error.Failure("FAIL", "Falhou"));

        var act = () => _ = result.Value;

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("The value of a failure result can't be accessed.");
    }

    [Fact]
    public void ImplicitConversion_WithNullValue_ShouldReturnNullValueFailure()
    {
        Result<string> result = (string?)null;

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Error.NullValue);
    }
}
