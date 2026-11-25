using YourBrand.TimeReport.Features.Tasks.TaskTypes;
using YourBrand.TimeReport.Features.Projects;
using YourBrand.TimeReport.Domain.Entities;
using YourBrand.TimeReport.Features.Tasks.TaskCharges;

namespace YourBrand.TimeReport.Features.Tasks;

public record class TaskDto(string Id, string Name, TaskTypeDto TaskType, string? Description, decimal? HourlyRate, IEnumerable<TaskChargeDto> Charge, ProjectDto? Project);