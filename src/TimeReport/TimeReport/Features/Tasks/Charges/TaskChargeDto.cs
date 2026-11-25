using YourBrand.TimeReport.Features.Projects;
using YourBrand.TimeReport.Domain.Entities;
using YourBrand.TimeReport.Features.Tasks;
using YourBrand.TimeReport.Features.Tasks.TaskTypes;

namespace YourBrand.TimeReport.Features.Tasks.TaskCharges;

public record class TaskChargeDto(
    string Id, 
    ChargeType Type,
    TaskDto? Task,
    TaskTypeDto? TaskType,
    ChargeBasis Basis,
    decimal Amount,
    string? Description);