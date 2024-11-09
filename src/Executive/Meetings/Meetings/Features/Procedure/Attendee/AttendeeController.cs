using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace YourBrand.Meetings.Features.Procedure.Attendee;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/Meetings")]
[Authorize]
public sealed partial class AttendeeController(IMediator mediator) : ControllerBase
{

}