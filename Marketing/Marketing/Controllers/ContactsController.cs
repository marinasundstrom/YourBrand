using YourBrand.Marketing.Application;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using YourBrand.Marketing.Application.Contacts.Queries;

namespace YourBrand.Marketing.Controllers;

[Route("[controller]")]
public class ContactsController : ControllerBase 
{
    private readonly IMediator _mediator;

    public ContactsController(IMediator mediator) 
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<ItemsResult<Application.Contacts.ContactDto>>> GetContacts(int page, int pageSize, CancellationToken cancellationToken = default) 
    {
        var result = await _mediator.Send(new GetContacts(page, pageSize), cancellationToken);
        return Ok(result);
    }
}
