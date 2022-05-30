using YourBrand.Customers.Application;
using YourBrand.Customers.Application.Queries;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace YourBrand.Customers.Controllers;

[Route("[controller]")]
public class PersonsController : ControllerBase 
{
    private readonly IMediator _mediator;

    public PersonsController(IMediator mediator) 
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<ItemsResult<PersonDto>>> GetPersons(int page, int pageSize, CancellationToken cancellationToken = default) 
    {
        var result = await _mediator.Send(new GetPersons(page, pageSize), cancellationToken);
        return Ok(result);
    }
}