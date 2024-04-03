using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using YourBrand.Customers.Application.Addresses;
using YourBrand.Customers.Application.Persons.Commands;
using YourBrand.Customers.Application.Persons.Queries;

namespace YourBrand.Customers.Application.Persons;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/[controller]")]
[Authorize]
public class PersonsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PersonsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<ItemsResult<Persons.PersonDto>>> GetPersons(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetPersons(page, pageSize), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<PersonDto?> GetPerson(int id, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new GetPerson(id), cancellationToken);
    }

    [HttpPost]
    [ProducesDefaultResponseType]
    [ProducesResponseType(typeof(PersonDto), StatusCodes.Status201Created)]
    public async Task<ActionResult> CreatePerson([FromBody] CreatePersonDto dto, CancellationToken cancellationToken)
    {
        var dto2 = await _mediator.Send(new CreatePerson(dto.FirstName, dto.LastName, dto.SSN, dto.Phone, dto.PhoneMobile, dto.Email, dto.Address), cancellationToken);
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

public record CreatePersonDto(string FirstName, string LastName, string SSN, string? Phone, string? PhoneMobile, string? Email, Address2Dto Address);

public record UpdatePersonDto(string FirstName, string LastName, string SSN, string? Phone, string? PhoneMobile, string? Email);