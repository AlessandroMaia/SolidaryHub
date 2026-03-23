namespace Identity.UnitTests.ServiceDefaults;

public sealed class JwtAndOpenApiTests
{
    [Fact]
    public async Task AddJwtAuthentication_WithValidSettings_ShouldRegisterJwtSchemeAndOptions()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddJwtAuthentication(CreateValidConfiguration());
        using var provider = services.BuildServiceProvider();

        var schemes = provider.GetRequiredService<IAuthenticationSchemeProvider>();
        var optionsMonitor = provider.GetRequiredService<IOptionsMonitor<JwtBearerOptions>>();
        var scheme = await schemes.GetSchemeAsync(JwtBearerDefaults.AuthenticationScheme);
        var options = optionsMonitor.Get(JwtBearerDefaults.AuthenticationScheme);
        var expiredContext = new AuthenticationFailedContext(
            new DefaultHttpContext(),
            scheme!,
            options)
        {
            Exception = new Microsoft.IdentityModel.Tokens.SecurityTokenExpiredException()
        };
        var defaultContext = new AuthenticationFailedContext(
            new DefaultHttpContext(),
            scheme!,
            options)
        {
            Exception = new InvalidOperationException("boom")
        };

        scheme.Should().NotBeNull();
        options.SaveToken.Should().BeTrue();
        options.RequireHttpsMetadata.Should().BeFalse();
        options.TokenValidationParameters.ValidateIssuerSigningKey.Should().BeTrue();
        options.TokenValidationParameters.ValidIssuer.Should().Be("issuer");
        options.TokenValidationParameters.ValidAudience.Should().Be("audience");

        await options.Events.OnAuthenticationFailed(expiredContext);
        expiredContext.Response.Headers["token-expired"].ToString().Should().Be("true");

        await options.Events.OnAuthenticationFailed(defaultContext);
        defaultContext.Response.Headers.Should().NotContainKey("token-expired");
        await options.Events.OnTokenValidated(null!);
        await options.Events.OnChallenge(null!);
    }

    [Fact]
    public void AddJwtAuthentication_WithInvalidSecret_ShouldThrow()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [$"{TokenSettings.SectionName}:Secret"] = "short",
                [$"{TokenSettings.SectionName}:Issuer"] = "issuer",
                [$"{TokenSettings.SectionName}:Audience"] = "audience"
            })
            .Build();

        var services = new ServiceCollection();

        var act = () => services.AddJwtAuthentication(configuration);

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void AddJwtAuthentication_WithoutSection_ShouldThrow()
    {
        var services = new ServiceCollection();

        var act = () => services.AddJwtAuthentication(new ConfigurationBuilder().Build());

        act.Should().Throw<InvalidOperationException>()
            .WithMessage($"A seção de configuração '{TokenSettings.SectionName}' não foi encontrada");
    }

    [Fact]
    public async Task BearerSecuritySchemeTransformer_WithJwtScheme_ShouldAddSecurityRequirements()
    {
        var provider = Substitute.For<IAuthenticationSchemeProvider>();
        provider.GetAllSchemesAsync().Returns([new AuthenticationScheme(JwtBearerDefaults.AuthenticationScheme, null, typeof(IAuthenticationHandler))]);
        var transformer = new BearerSecuritySchemeTransformer(provider);
        var pathItem = new OpenApiPathItem();
        var addOperation = typeof(OpenApiPathItem).GetMethods()
            .Single(method => method.Name == "AddOperation");
        var operationIdentifierType = addOperation.GetParameters()[0].ParameterType;
        var operationIdentifier = operationIdentifierType == typeof(string)
            ? "get"
            : operationIdentifierType == typeof(System.Net.Http.HttpMethod)
                ? System.Net.Http.HttpMethod.Get
            : operationIdentifierType.IsEnum
                ? Enum.Parse(operationIdentifierType, "Get")
                : throw new InvalidOperationException(
                    $"Tipo de identificador de operacao nao suportado: {operationIdentifierType.FullName}");

        addOperation.Invoke(pathItem, [operationIdentifier, new OpenApiOperation()]);

        var document = new OpenApiDocument
        {
            Paths = new OpenApiPaths
            {
                ["/secure"] = pathItem
            }
        };

        await transformer.TransformAsync(document, null!, CancellationToken.None);

        document.Components.Should().NotBeNull();
        document.Components!.SecuritySchemes.Should().ContainKey(JwtBearerDefaults.AuthenticationScheme);
        var operations = (System.Collections.IEnumerable)typeof(OpenApiPathItem)
            .GetProperty(nameof(OpenApiPathItem.Operations))!
            .GetValue(document.Paths["/secure"])!;
        var operation = operations.Cast<object>()
            .Select(item => item.GetType().GetProperty("Value")!.GetValue(item))
            .Cast<OpenApiOperation>()
            .Single();

        operation.Security.Should().NotBeEmpty();
    }

    [Fact]
    public async Task BearerSecuritySchemeTransformer_WithoutJwtScheme_ShouldDoNothing()
    {
        var provider = Substitute.For<IAuthenticationSchemeProvider>();
        provider.GetAllSchemesAsync().Returns([new AuthenticationScheme("Cookies", null, typeof(IAuthenticationHandler))]);
        var transformer = new BearerSecuritySchemeTransformer(provider);
        var document = new OpenApiDocument();

        await transformer.TransformAsync(document, null!, CancellationToken.None);

        document.Components.Should().BeNull();
    }

    [Fact]
    public async Task BearerSecuritySchemeTransformer_WithPathWithoutOperations_ShouldKeepDocumentValid()
    {
        var provider = Substitute.For<IAuthenticationSchemeProvider>();
        provider.GetAllSchemesAsync().Returns([new AuthenticationScheme(JwtBearerDefaults.AuthenticationScheme, null, typeof(IAuthenticationHandler))]);
        var transformer = new BearerSecuritySchemeTransformer(provider);
        var document = new OpenApiDocument
        {
            Paths = new OpenApiPaths
            {
                ["/empty"] = new OpenApiPathItem()
            }
        };

        await transformer.TransformAsync(document, null!, CancellationToken.None);

        document.Components.Should().NotBeNull();
        document.Components!.SecuritySchemes.Should().ContainKey(JwtBearerDefaults.AuthenticationScheme);
    }

    private static IConfiguration CreateValidConfiguration()
        => new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [$"{TokenSettings.SectionName}:Secret"] = "12345678901234567890123456789012",
                [$"{TokenSettings.SectionName}:Issuer"] = "issuer",
                [$"{TokenSettings.SectionName}:Audience"] = "audience",
                [$"{TokenSettings.SectionName}:AccessTokenExpirationMinutes"] = "15"
            })
            .Build();
}
