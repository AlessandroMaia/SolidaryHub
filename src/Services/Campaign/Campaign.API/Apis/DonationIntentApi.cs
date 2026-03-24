using Campaign.API.Application.Commands.DonationIntents.CreateDonationIntent;
using Campaign.API.Application.Commands.DonationIntents.ProcessDonationIntent;
using Campaign.API.Application.Commands.DonationIntents.RejectDonationIntent;
using Campaign.API.Application.Queries.DonationIntents.GetDonationIntentById;
using Campaign.API.Application.Queries.DonationIntents.GetDonationIntentsByCampaign;
using Campaign.API.Application.Queries.DonationIntents.GetDonationIntentsByDonor;
using Campaign.API.Application.Queries.DonationIntents.GetPendingDonationIntents;
using Campaign.Domain.Services;

namespace Campaign.API.Apis;

public static class DonationIntentApi
{
    public static RouteGroupBuilder MapDonationIntentApi(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/donation-intents")
            .WithTags("Intenção de doação");

        var donorGroup = group.MapGroup(string.Empty)
            .RequireDonorAccess();

        var appUserGroup = group.MapGroup(string.Empty)
            .RequireApplicationUserAccess();

        var managerGroup = group.MapGroup(string.Empty)
            .RequireManagerAccess();

        donorGroup.MapPost("/{campaignId:int}", Create)
            .WithName("CadastrarIntencaoDeDoacao")
            .WithSummary("Cadastra uma nova intenção de doação")
            .WithDescription("Cria uma nova intenção de doação para a campanha informada.")
            .Produces(StatusCodes.Status201Created)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status403Forbidden)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized);

        appUserGroup.MapGet("/{id:int}", GetById)
            .WithName("ObterIntencaoDeDoacaoPorId")
            .WithSummary("Obtém uma intenção de doação por identificador")
            .WithDescription("Retorna os detalhes de uma intenção de doação específica.")
            .Produces<DonationIntentDetailsViewModel>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status403Forbidden)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized);

        appUserGroup.MapGet("/donors/{donorId:int}", GetByDonorId)
            .WithName("ObterIntencoesDeDoacaoPorDoador")
            .WithSummary("Obtém as intenções de doação de um doador")
            .WithDescription("Retorna a lista de intenções de doação vinculadas ao doador informado.")
            .Produces<PagedResponse<DonationIntentListItemViewModel>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status403Forbidden)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized);

        managerGroup.MapPut("/{id:int}/process", Process)
            .WithName("ProcessarIntencaoDeDoacao")
            .WithSummary("Processa uma intenção de doação")
            .WithDescription("Processa uma intenção de doação existente.")
            .Produces(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized);

        managerGroup.MapPut("/{id:int}/reject", Reject)
            .WithName("RejeitarIntencaoDeDoacao")
            .WithSummary("Rejeita uma intenção de doação")
            .WithDescription("Rejeita uma intenção de doação existente informando o motivo.")
            .Produces(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized);

        managerGroup.MapGet("/campaigns/{campaignId:int}", GetByCampaignId)
            .WithName("ObterIntencoesDeDoacaoPorCampanha")
            .WithSummary("Obtém as intenções de doação de uma campanha")
            .WithDescription("Retorna a lista de intenções de doação vinculadas à campanha informada.")
            .Produces<PagedResponse<DonationIntentListItemViewModel>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized);

        managerGroup.MapGet("/pendings", GetPendings)
            .WithName("ObterIntencoesDeDoacaoPendentes")
            .WithSummary("Obtém as intenções de doação pendentes")
            .WithDescription("Retorna a lista de intenções de doação com status pendente.")
            .Produces<PagedResponse<DonationIntentListItemViewModel>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized);

        return group;
    }

    private static async Task<IResult> Create(
        [FromHeader(Name = "x-requestid")] Guid? requestId,
        [FromHeader(Name = "x-correlation-id")] string? correlationId,
        [FromRoute] int campaignId,
        [FromBody] CreateDonationIntentRequestViewModel request,
        HttpContext httpContext,
        IIdentityService identityService,
        IMediator mediator,
        CancellationToken ct)
    {
        var donorIdString = identityService.GetUserIdentity();

        if (string.IsNullOrEmpty(donorIdString) || !int.TryParse(donorIdString, out var donorId))
            return Results.Unauthorized();

        var effectiveRequestId = requestId.GetValueOrDefault(Guid.NewGuid());
        var effectiveCorrelationId = correlationId ?? httpContext.TraceIdentifier;

        var command = new CreateDonationIntentCommand(
            campaignId,
            donorId,
            request.Amount,
            request.Currency,
            "api",
            effectiveCorrelationId,
            null,
            effectiveRequestId);

        var id = await mediator.Send(command, ct);
        return TypedResults.Created($"/api/donation-intents/{id}");
    }

    private static async Task<IResult> Process(
        [FromHeader(Name = "x-requestid")] Guid? requestId,
        [FromRoute] int id,
        [FromBody] DonationIntentProcessRequestViewModel request,
        IMediator mediator,
        CancellationToken ct)
    {
        var effectiveRequestId = requestId.GetValueOrDefault(Guid.NewGuid());
        var command = new ProcessDonationIntentCommand(id, request.WorkerName, effectiveRequestId);

        await mediator.Send(command, ct);
        return Results.Ok();
    }

    private static async Task<IResult> Reject(
        [FromHeader(Name = "x-requestid")] Guid? requestId,
        [FromRoute] int id,
        [FromBody] DonationIntentRejectRequestViewModel request,
        IMediator mediator,
        CancellationToken ct)
    {
        var effectiveRequestId = requestId.GetValueOrDefault(Guid.NewGuid());
        var command = new RejectDonationIntentCommand(id, request.Reason, effectiveRequestId);

        await mediator.Send(command, ct);
        return Results.Ok();
    }

    private static async Task<IResult> GetById(
        [FromRoute] int id,
        IIdentityService identityService,
        IMediator mediator,
        CancellationToken ct)
    {
        var query = new GetDonationIntentByIdQuery(id);
        var result = await mediator.Receive(query, ct);

        if (result is null)
        {
            return TypedResults.NotFound(new ProblemDetails
            {
                Title = "Intenção de doação não encontrada",
                Detail = $"Intenção de doação com ID {id} não existe.",
                Status = StatusCodes.Status404NotFound
            });
        }

        var userIdString = identityService.GetUserIdentity();

        if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var currentUserId))
            return TypedResults.Unauthorized();

        var isManager = identityService.IsInRole(Roles.Manager);

        if (!isManager && result.DonorUserId != currentUserId)
            return TypedResults.Forbid();

        return TypedResults.Ok(result);
    }

    private static async Task<IResult> GetByCampaignId(
        [FromRoute] int campaignId,
        IMediator mediator,
        CancellationToken ct,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetDonationIntentsByCampaignQuery(campaignId, page, pageSize);
        var result = await mediator.Receive(query, ct);
        return TypedResults.Ok(result);
    }

    private static async Task<IResult> GetByDonorId(
        [FromRoute] int donorId,
        IIdentityService identityService,
        IMediator mediator,
        CancellationToken ct,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var userIdString = identityService.GetUserIdentity();

        if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var currentUserId))
            return TypedResults.Unauthorized();

        var isManager = identityService.IsInRole(Roles.Manager);

        if (!isManager && donorId != currentUserId)
            return TypedResults.Forbid();

        var query = new GetDonationIntentsByDonorQuery(donorId, page, pageSize);
        var result = await mediator.Receive(query, ct);
        return TypedResults.Ok(result);
    }

    private static async Task<IResult> GetPendings(
        IMediator mediator,
        CancellationToken ct,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetPendingDonationIntentsQuery(page, pageSize);
        var result = await mediator.Receive(query, ct);
        return TypedResults.Ok(result);
    }
}
