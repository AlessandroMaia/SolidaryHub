using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SharedKernel;

namespace ServiceDefaults;

public static class JwtAuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<TokenSettings>(
            configuration.GetSection(TokenSettings.SectionName));

        var tokenSettings = configuration
            .GetSection(TokenSettings.SectionName)
            .Get<TokenSettings>()
            ?? throw new InvalidOperationException(
                $"A seção de configuração '{TokenSettings.SectionName}' não foi encontrada");

        if (string.IsNullOrWhiteSpace(tokenSettings.Secret) || tokenSettings.Secret.Length < 32)
            throw new InvalidOperationException("A chave secreta do token deve conter no mínimo 32 caracteres");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSettings.Secret)),

                ValidateIssuer = true,
                ValidIssuer = tokenSettings.Issuer,

                ValidateAudience = true,
                ValidAudience = tokenSettings.Audience,

                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,

                NameClaimType = ClaimTypes.Name,
                RoleClaimType = ClaimTypes.Role
            };

            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    if (context.Exception is SecurityTokenExpiredException)
                        context.Response.Headers.Add(
                            new KeyValuePair<string, Microsoft.Extensions.Primitives.StringValues>(
                                "token-expired",
                                "true"));

                    return Task.CompletedTask;
                },
                OnTokenValidated = _ => Task.CompletedTask,
                OnChallenge = _ => Task.CompletedTask
            };
        });

        services.AddAuthorization();

        return services;
    }
}