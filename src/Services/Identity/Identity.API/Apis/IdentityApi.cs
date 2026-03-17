using Identity.API.Application.Commands.ChangePassword;
using Identity.API.Application.Commands.RefreshToken;
using Identity.API.Application.Commands.RegisterUser;
using Identity.API.Application.Commands.SignIn;
using Identity.API.Application.Queries.GetUserById;

namespace Identity.API.Apis;

public static class IdentityApi
{
    public static IEndpointRouteBuilder MapIdentityEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/user")
            .WithTags("Autenticação");

        group.MapPost("/register", RegisterAsync)
            .WithName("CadastrarUsuario")
            .WithSummary("Cadastra um novo usuário")
            .WithDescription("Cria uma nova conta de usuário com e-mail e senha")
            .Produces<RegisterUserViewModel>(StatusCodes.Status201Created)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .AllowAnonymous();

        group.MapPost("/sign-in", SignInAsync)
            .WithName("AutenticarUsuario")
            .WithSummary("Autentica um usuário")
            .WithDescription("Autentica o usuário e retorna o token de acesso JWT e o token de atualização")
            .Produces<SignInViewModel>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .AllowAnonymous();

        group.MapPost("/refresh", RefreshTokenAsync)
            .WithName("RenovarToken")
            .WithSummary("Renova o token de acesso")
            .WithDescription("Gera um novo token de acesso utilizando o token de atualização")
            .Produces<SignInViewModel>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .AllowAnonymous();

        group.MapPut("/change-password", ChangePasswordAsync)
            .WithName("AlterarSenha")
            .WithSummary("Altera a senha do usuário")
            .WithDescription("Altera a senha do usuário autenticado")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization("RequireAuthenticatedUser");

        group.MapGet("/validate", ValidateTokenAsync)
            .WithName("ValidarToken")
            .WithSummary("Valida o token JWT")
            .WithDescription("Verifica se o token JWT atual é válido e retorna as informações do usuário")
            .Produces<TokenValidationViewModel>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization("RequireAuthenticatedUser");

        group.MapGet("/me", GetCurrentUserAsync)
            .WithName("ObterUsuarioAtual")
            .WithSummary("Obtém o usuário autenticado atual")
            .WithDescription("Retorna as informações de perfil do usuário autenticado")
            .Produces<UserViewModel>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization("RequireAuthenticatedUser");

        group.MapGet("/{id:int}", GetUserByIdAsync)
            .WithName("ObterUsuarioPorId")
            .WithSummary("Obtém um usuário por ID")
            .WithDescription("Retorna as informações do usuário pelo ID (apenas administrador ou o próprio usuário)")
            .Produces<UserViewModel>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization("RequireAuthenticatedUser");

        return group;
    }

    private static async Task<IResult> RegisterAsync(
        [FromBody] RegisterUserCommand command,
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return Results.Created($"/api/users/{result.UserId}", result);
    }

    private static async Task<IResult> SignInAsync(
        [FromBody] SignInCommand command,
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return Results.Ok(result);
    }

    private static async Task<IResult> RefreshTokenAsync(
        [FromBody] RefreshTokenCommand command,
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return Results.Ok(result);
    }

    private static async Task<IResult> ChangePasswordAsync(
        [FromBody] ChangePasswordCommand command,
        IMediator mediator,
        IIdentityService identityService,
        CancellationToken ct)
    {
        var userIdString = identityService.GetUserIdentity();

        if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
            return Results.Unauthorized();

        await mediator.Send(command, ct);

        return Results.NoContent();
    }

    private static IResult ValidateTokenAsync(IIdentityService identityService)
    {
        if (!identityService.IsAuthenticated())
            return Results.Unauthorized();

        var response = new TokenValidationViewModel(
            identityService.GetUserIdentity()!,
            identityService.GetUserName(),
            identityService.GetUserEmail(),
            identityService.GetUserRoles(),
            true
        );

        return Results.Ok(response);
    }

    private static async Task<IResult> GetCurrentUserAsync(
        IIdentityService identityService,
        IMediator mediator,
        CancellationToken ct)
    {
        var userIdString = identityService.GetUserIdentity();

        if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
            return TypedResults.Unauthorized();

        var query = new GetUserByIdQuery(userId);
        var user = await mediator.Receive(query, ct);

        if (user is null)
            return TypedResults.NotFound();

        return TypedResults.Ok(user);
    }

    private static async Task<IResult> GetUserByIdAsync(
        int id,
        IIdentityService identityService,
        IMediator mediator,
        CancellationToken ct)
    {
        var userIdString = identityService.GetUserIdentity();

        if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var currentUserId))
            return TypedResults.Forbid();

        var isAdmin = identityService.IsInRole(Role.Roles.Manager);
        var isOwnProfile = currentUserId == id;

        if (!isAdmin && !isOwnProfile)
            return TypedResults.Forbid();

        var query = new GetUserByIdQuery(id);
        var user = await mediator.Receive(query, ct);

        if (user is null)
            return TypedResults.NotFound();

        return TypedResults.Ok(user);
    }
}
