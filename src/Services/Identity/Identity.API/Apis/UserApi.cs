using Identity.API.Application.Queries.GetUserById;

namespace Identity.API.Apis;

public static class UserApi
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users")
            .WithTags("Usuários")
            .RequireApplicationUserAccess();

        group.MapGet("/me", GetCurrentUserAsync)
            .WithName("ObterUsuarioAtual")
            .WithSummary("Obtém o usuário autenticado atual")
            .WithDescription("Retorna as informações de perfil do usuário autenticado")
            .Produces<UserViewModel>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/{id:int}", GetUserByIdAsync)
            .WithName("ObterUsuarioPorId")
            .WithSummary("Obtém um usuário por ID")
            .WithDescription("Retorna as informações do usuário pelo ID (apenas administrador ou o próprio usuário)")
            .Produces<UserViewModel>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound);

        return group;
    }

    private static async Task<IResult> GetCurrentUserAsync(
        IIdentityService identityService,
        IMediator mediator,
        CancellationToken ct)
    {
        var userIdString = identityService.GetUserIdentity();

        if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
            return Results.Unauthorized();

        var query = new GetUserByIdQuery(userId);
        var user = await mediator.Receive(query, ct);

        if (user is null)
            return Results.NotFound();

        return Results.Ok(user);
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

        var isAdmin = identityService.IsInRole(Roles.Manager);
        var isOwnProfile = currentUserId == id;

        if (!isAdmin && !isOwnProfile)
            return Results.Forbid();

        var query = new GetUserByIdQuery(id);
        var user = await mediator.Receive(query, ct);

        if (user is null)
            return Results.NotFound();

        return Results.Ok(user);
    }
}
