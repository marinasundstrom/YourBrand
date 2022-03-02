
using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Skynet.TimeReport.Application.Common.Interfaces;
using Skynet.TimeReport.Application.Reports.Queries;
using Skynet.TimeReport.Infrastructure;

namespace Skynet.TimeReport.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReportsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<FileStreamResult> GetReport([FromQuery] string[] projectIds, [FromQuery] string? userId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] int[] statuses, ReportModeDto mode = ReportModeDto.Project, CancellationToken cancellationToken = default)
    {
        var stream = await _mediator.Send(new CreateReportCommand(projectIds, userId, startDate, endDate, statuses, (ReportMode?)mode ?? ReportMode.Project), cancellationToken);

        if (stream is null) throw new Exception();

        return File(stream, "application/vnd.ms-excel", "TimeReport.xlsx");
    }
}

public enum ReportModeDto
{
    User,
    Project
}