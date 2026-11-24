using YourBrand.TimeReport.Application.Projects;
using YourBrand.TimeReport.Domain.Entities;
using YourBrand.TimeReport.Application.Tasks;
using YourBrand.TimeReport.Application.Tasks.TaskTypes;

namespace YourBrand.TimeReport.Application.Tasks.TaskCharges;

public record class TaskChargeDto(
    string Id, 
    ChargeType Type,
    TaskDto? Task,
    TaskTypeDto? TaskType,
    ChargeBasis Basis,
    decimal Amount,
    string? Description);