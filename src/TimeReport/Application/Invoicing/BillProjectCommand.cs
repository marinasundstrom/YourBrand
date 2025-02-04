using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Invoicing.Client;
using YourBrand.TimeReport.Application.Common.Interfaces;

namespace YourBrand.TimeReport.Application.Invoicing;

public record BillProjectCommand(string OrganizationId, string ProjectId, DateTime From, DateTime To) : IRequest<Result>
{
    public class Handler(ITimeReportContext context, IInvoicesClient invoicesClient) : IRequestHandler<BillProjectCommand, Result>
    {
        public async Task<Result> Handle(BillProjectCommand request, CancellationToken cancellationToken)
        {
            var from = DateOnly.FromDateTime(request.From);
            var to = DateOnly.FromDateTime(request.To);

            var entries = await context.Entries
                .InOrganization(request.OrganizationId)
                .Include(e => e.Project)
                .Include(e => e.Task)
                .AsSplitQuery()
                .Where(e => e.Project.Id == request.ProjectId)
                .Where(e => e.Date > from && e.Date < to)
                .Where(e => e.TimeSheet.Status == Domain.Entities.TimeSheetStatus.Approved)
                .ToArrayAsync(cancellationToken);

            var entriesByTask = entries.GroupBy(x => x.Task);

            if (!entries.Any())
            {
                return Result.Success;
            }

            var invoice = await invoicesClient.CreateInvoiceAsync(new CreateInvoice()
            {
                OrganizationId = request.OrganizationId,
                Date = DateTime.Now,
                Note = entriesByTask.First().Key.Project.Name
            });

            foreach (var entryGroup in entriesByTask)
            {
                var description = entryGroup.Key.Name;
                var hourlyRate = entryGroup.Key.HourlyRate.GetValueOrDefault();

                var hours = entryGroup.Sum(e => e.Hours.GetValueOrDefault());

                await invoicesClient.AddItemAsync(
                    request.OrganizationId,
                    invoice.Id,
                    new AddInvoiceItem
                    {
                        ProductType = ProductType.Service,
                        Description = description,
                        UnitPrice = hourlyRate.GetVatFromTotal(0.25),
                        Unit = "hours",
                        VatRate = 0.25,
                        Quantity = hours
                    });
            }

            // TODO: Mark TimeSheets as billed.

            return Result.Success;
        }
    }
}