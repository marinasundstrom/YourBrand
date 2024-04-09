using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using YourBrand.Marketing.Application.Contacts.Commands;
using YourBrand.Marketing.Application.Contacts.Queries;

namespace YourBrand.Marketing.Application.Contacts;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/[controller]")]
public class ContactsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ItemsResult<ContactDto>> GetContacts(int page = 1, int pageSize = 10, string? campaignId = null, string? searchString = null, string? sortBy = null, Application.Common.Models.SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        return await mediator.Send(new GetContacts(page, pageSize, campaignId, searchString, sortBy, sortDirection), cancellationToken);
    }

    [HttpGet("{id}")]
    public async Task<ContactDto?> GetContact(string id, CancellationToken cancellationToken)
    {
        return await mediator.Send(new GetContact(id), cancellationToken);
    }

    [HttpPost]
    public async Task<ContactDto> CreateContact(CreateContactDto dto, CancellationToken cancellationToken)
    {
        return await mediator.Send(new CreateContact(dto.FirstName, dto.LastName, dto.SSN, dto.CampaignId), cancellationToken);
    }

    [HttpPut("{id}")]
    public async Task UpdateContact(string id, UpdateContactDto dto, CancellationToken cancellationToken)
    {
        await mediator.Send(new UpdateContact(id, dto.FirstName, dto.LastName, dto.SSN, dto.CampaignId), cancellationToken);
    }

    [HttpDelete("{id}")]
    public async Task DeleteContact(string id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteContact(id), cancellationToken);
    }
}

public record CreateContactDto(string FirstName, string LastName, string SSN, string CampaignId);

public record UpdateContactDto(string FirstName, string LastName, string SSN, string CampaignId);