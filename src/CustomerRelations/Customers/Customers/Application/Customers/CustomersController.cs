using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using YourBrand.Customers.Application.Customers.Queries;
using YourBrand.Customers.Features.Customers.Import;

namespace YourBrand.Customers.Application.Customers;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/[controller]")]
public class CustomersController(IMediator mediator)
  : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ItemsResult<Customers.CustomerDto>>> GetCustomers(int page, int pageSize, string? searchString, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetCustomers(page, pageSize, searchString), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<CustomerDto?> GetCustomer(int id, CancellationToken cancellationToken)
    {
        return await mediator.Send(new GetCustomer(id), cancellationToken);
    }


    [HttpGet("GetCustomerBySsn/{ssn}")]
    public async Task<CustomerDto?> GetCustomerBySSN(string ssn, CancellationToken cancellationToken)
    {
        return await mediator.Send(new GetCustomerBySSN(ssn), cancellationToken);
    }

    [HttpPost("ImportCustomers")]
    [ProducesResponseType(typeof(CustomerImportResult), StatusCodes.Status200OK)]
    public async Task<ActionResult> ImportCustomers(IFormFile file, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new ImportCustomers(file.OpenReadStream()), cancellationToken);
        return this.HandleResult(result);
    }
}