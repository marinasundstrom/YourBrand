using YourBrand.Customers.Application;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using YourBrand.Customers.Application.Persons.Queries;
using YourBrand.Customers.Application.Persons;
using YourBrand.Customers.Application.Persons.Commands;

namespace YourBrand.Customers.Controllers;

[Route("Customers/[controller]")]
public class PersonsController : ControllerBase 
{
    private readonly IMediator _mediator;

    public PersonsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<ItemsResult<Application.Persons.PersonDto>>> GetPersons(int page, int pageSize, CancellationToken cancellationToken = default) 
    {
        var result = await _mediator.Send(new GetPersons(page, pageSize), cancellationToken);
        return Ok(result);
    }
    
    [HttpGet("{id}")]
    public async Task<PersonDto?> GetPerson(string id, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new GetPerson(id), cancellationToken);
    }

    [HttpPost]
    [ProducesDefaultResponseType]
    [ProducesResponseType(typeof(PersonDto), StatusCodes.Status201Created)]
    public async Task<ActionResult> CreatePerson([FromBody] CreatePersonDto dto, CancellationToken cancellationToken)
    {
        var dto2 = await _mediator.Send(new CreatePerson(dto.FirstName, dto.LastName, dto.SSN, dto.Phone, dto.PhoneMobile, dto.Email), cancellationToken);
        return CreatedAtAction(nameof(GetPerson), new { id = dto2.Id }, dto2);
    }

    [HttpPut("{id}")]
    public async Task UpdatePerson(string id, [FromBody] UpdatePersonDto dto, CancellationToken cancellationToken)
    {
        //await _mediator.Send(new UpdatePerson(dto.FirstName, dto.LastName, dto.SSN), cancellationToken);
    }

    [HttpDelete("{id}")]
    public async Task DeletePerson(string id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeletePerson(id), cancellationToken);
    }
}

public record CreatePersonDto(string FirstName, string LastName, string SSN, string? Phone, string? PhoneMobile, string? Email);

public record UpdatePersonDto(string FirstName, string LastName, string SSN, string? Phone, string? PhoneMobile, string? Email);
