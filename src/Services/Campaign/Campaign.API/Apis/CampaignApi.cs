using Campaign.API.Application.Commands.Campaigns.CancelCampaign;
using Campaign.API.Application.Commands.Campaigns.CompleteCampaign;
using Campaign.API.Application.Commands.Campaigns.CreateCampaign;
using Campaign.API.Application.Commands.Campaigns.UpdateCampaign;
using Campaign.API.Application.Queries.Campaigns.GetActiveCampaigns;
using Campaign.API.Application.Queries.Campaigns.GetCampaignById;
using Campaign.API.Application.Queries.Campaigns.GetCampaignPublicPanel;
using Campaign.API.Application.Queries.Campaigns.GetCampaigns;
using Campaign.Domain.Services;

namespace Campaign.API.Apis;

public static class CampaignApi
{
    public static RouteGroupBuilder MapCampaignApi(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/campaigns")
            .WithTags("Campanhas");

        var managerGroup = group.MapGroup(string.Empty)
            .RequireManagerAccess();

        var publicGroup = group.MapGroup(string.Empty);

        managerGroup.MapPost("/", Create)
            .WithName("CadastrarCampanha")
            .WithSummary("Cadastra uma nova campanha social")
            .WithDescription("Cria uma nova campanha social com título, descrição, período e meta financeira.")
            .Produces(StatusCodes.Status201Created)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized);

        managerGroup.MapPut("/{id:int}", Update)
            .WithName("AtualizarCampanha")
            .WithSummary("Atualiza uma campanha")
            .WithDescription("Atualiza os dados de uma campanha existente.")
            .Produces(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized);

        managerGroup.MapPut("/{id:int}/complete", Complete)
            .WithName("ConcluirCampanha")
            .WithSummary("Conclui uma campanha")
            .WithDescription("Altera o status de uma campanha para concluída.")
            .Produces(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized);

        managerGroup.MapPut("/{id:int}/cancel", Cancel)
            .WithName("CancelarCampanha")
            .WithSummary("Cancela uma campanha")
            .WithDescription("Altera o status de uma campanha para cancelada.")
            .Produces(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized);

        managerGroup.MapGet("/", Get)
            .WithName("ObterCampanhas")
            .WithSummary("Obtém todas as campanhas")
            .WithDescription("Retorna a lista de campanhas cadastradas.")
            .Produces<PagedResponse<CampaignListItemViewModel>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized);

        publicGroup.MapGet("/public-panel", GetPublicPanel)
            .WithName("ObterPainelPublicoDeCampanhas")
            .WithSummary("Obtém o painel público de campanhas")
            .WithDescription("Retorna as campanhas disponíveis para exibição pública.")
            .Produces<PagedResponse<CampaignPublicPanelViewModel>>(StatusCodes.Status200OK);

        managerGroup.MapGet("/{id:int}", GetById)
            .WithName("ObterCampanhaPorId")
            .WithSummary("Obtém uma campanha por identificador")
            .WithDescription("Retorna os detalhes de uma campanha específica.")
            .Produces<CampaignDetailsViewModel>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized);

        publicGroup.MapGet("/active", GetActive)
            .WithName("ObterCampanhasAtivas")
            .WithSummary("Obtém as campanhas ativas")
            .WithDescription("Retorna a lista de campanhas com status ativo.")
            .Produces<PagedResponse<CampaignListItemViewModel>>(StatusCodes.Status200OK);

        return group;
    }

    private static async Task<IResult> Create(
        [FromHeader(Name = "x-requestid")] Guid? requestId,
        [FromBody] CreateCampaignResquestViewModel request,
        IIdentityService identityService,
        IMediator mediator,
        CancellationToken ct)
    {
        var userIdString = identityService.GetUserIdentity();

        if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
            return Results.Unauthorized();

        var effectiveRequestId = requestId.GetValueOrDefault(Guid.NewGuid());

        var command = new CreateCampaignCommand(
            userId,
            request.Title,
            request.Description,
            request.StartDate.UtcDateTime,
            request.EndDate.UtcDateTime,
            request.FinancialGoalAmount,
            effectiveRequestId);

        var id = await mediator.Send(command, ct);

        return TypedResults.Created($"/api/campaigns/{id}");
    }

    private static async Task<IResult> Update(
        [FromHeader(Name = "x-requestid")] Guid? requestId,
        [FromRoute] int id,
        [FromBody] UpdateCampaignRequestViewModel request,
        IMediator mediator,
        CancellationToken ct)
    {
        var effectiveRequestId = requestId.GetValueOrDefault(Guid.NewGuid());

        var command = new UpdateCampaignCommand(
            id,
            request.Title,
            request.Description,
            request.StartDate.UtcDateTime,
            request.EndDate.UtcDateTime,
            request.FinancialGoalAmount,
            effectiveRequestId);

        await mediator.Send(command, ct);

        return Results.Ok();
    }

    private static async Task<IResult> Complete(
        [FromHeader(Name = "x-requestid")] Guid? requestId,
        [FromRoute] int id,
        [FromBody] CampaignReasonRequestViewModel request,
        IIdentityService identityService,
        IMediator mediator,
        CancellationToken ct)
    {
        var userIdString = identityService.GetUserIdentity();

        if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
            return Results.Unauthorized();

        var effectiveRequestId = requestId.GetValueOrDefault(Guid.NewGuid());
        var command = new CompleteCampaignCommand(id, userId, request.Reason, effectiveRequestId);

        await mediator.Send(command, ct);
        return Results.Ok();
    }

    private static async Task<IResult> Cancel(
        [FromHeader(Name = "x-requestid")] Guid? requestId,
        [FromRoute] int id,
        [FromBody] CampaignReasonRequestViewModel request,
        IIdentityService identityService,
        IMediator mediator,
        CancellationToken ct)
    {
        var userIdString = identityService.GetUserIdentity();

        if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
            return Results.Unauthorized();

        var effectiveRequestId = requestId.GetValueOrDefault(Guid.NewGuid());
        var command = new CancelCampaignCommand(id, userId, request.Reason, effectiveRequestId);

        await mediator.Send(command, ct);
        return TypedResults.Ok();
    }

    private static async Task<IResult> Get(
        IMediator mediator,
        CancellationToken ct,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetCampaignsQuery(page, pageSize);
        var result = await mediator.Receive(query, ct);
        return TypedResults.Ok(result);
    }

    private static async Task<IResult> GetPublicPanel(
        IMediator mediator,
        CancellationToken ct,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetCampaignPublicPanelQuery(page, pageSize);
        var result = await mediator.Receive(query, ct);
        return TypedResults.Ok(result);
    }

    private static async Task<IResult> GetById(
        [FromRoute] int id,
        IMediator mediator,
        CancellationToken ct)
    {
        var query = new GetCampaignByIdQuery(id);
        var result = await mediator.Receive(query, ct);

        return result is not null
            ? TypedResults.Ok(result)
            : TypedResults.NotFound(new ProblemDetails
            {
                Title = "Campanha não encontrada",
                Detail = $"Campanha com ID {id} não existe.",
                Status = StatusCodes.Status404NotFound
            });
    }

    private static async Task<IResult> GetActive(
        IMediator mediator,
        CancellationToken ct,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetActiveCampaignsQuery(page, pageSize);
        var result = await mediator.Receive(query, ct);
        return TypedResults.Ok(result);
    }
}
