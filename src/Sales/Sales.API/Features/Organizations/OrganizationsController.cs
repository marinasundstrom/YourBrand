namespace YourBrand.Sales.Features.Features.Organizations;

/*
[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/[controller]")]
public sealed class OrganizationsController : ControllerBase
{
    private readonly IMediator mediator;

    public OrganizationsController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResult<OrganizationDto>))]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesDefaultResponseType]
    public async Task<PagedResult<OrganizationDto>> GetOrganizations(int page = 1, int pageSize = 10, string? searchTerm = null, string? sortBy = null, SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
        => await mediator.Send(new GetOrganizations(page, pageSize, searchTerm, sortBy, sortDirection), cancellationToken);

    [HttpGet("OrganizationInfo")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrganizationDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<OrganizationDto>> GetOrganizationInfo(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetOrganizationInfo(), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrganizationDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<OrganizationDto>> CreateOrganization(CreateOrganizationDto request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreateOrganization(request.Name, request.Email), cancellationToken);
        return this.HandleResult(result);
    }
}

public sealed record CreateOrganizationDto(string Name, string Email);
*/