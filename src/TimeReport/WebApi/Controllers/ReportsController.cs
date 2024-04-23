using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using YourBrand.ApiKeys;
using YourBrand.TimeReport.Application.Reports.Queries;

namespace YourBrand.TimeReport.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class ReportsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<FileStreamResult> GetReport([FromQuery] string[] projectIds, [FromQuery] string? userId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] int[] statuses, ReportModeDto mode = ReportModeDto.Project, CancellationToken cancellationToken = default)
    {
        var stream = await mediator.Send(new CreateReportCommand(projectIds, userId, startDate, endDate, statuses, (ReportMode?)mode ?? ReportMode.Project), cancellationToken);

        if (stream is null) throw new Exception();

        return File(stream, "application/vnd.ms-excel", "TimeReport.xlsx");
    }
}

public enum ReportModeDto
{
    User,
    Project
}