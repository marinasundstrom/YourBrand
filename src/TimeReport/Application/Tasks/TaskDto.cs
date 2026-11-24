using YourBrand.TimeReport.Application.Tasks.TaskTypes;
using YourBrand.TimeReport.Application.Projects;
using YourBrand.TimeReport.Domain.Entities;
using YourBrand.TimeReport.Application.Tasks.TaskCharges;

namespace YourBrand.TimeReport.Application.Tasks;

public record class TaskDto(string Id, string Name, TaskTypeDto TaskType, string? Description, decimal? HourlyRate, IEnumerable<TaskChargeDto> Charge, ProjectDto? Project);