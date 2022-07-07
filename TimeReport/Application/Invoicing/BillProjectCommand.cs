using System;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Invoicing.Client;
using YourBrand.TimeReport.Application.Common.Interfaces;

namespace YourBrand.TimeReport.Application.Invoicing;

public record BillProjectCommand(string ProjectId, DateTime From, DateTime To) : IRequest
{
    public class Handler : IRequestHandler<BillProjectCommand>
    {
        private readonly ITimeReportContext _context;
        private readonly IInvoicesClient _invoicesClient;

        public Handler(ITimeReportContext context, IInvoicesClient invoicesClient)
        {
            _context = context;
            _invoicesClient = invoicesClient;
        }

        public async Task<MediatR.Unit> Handle(BillProjectCommand request, CancellationToken cancellationToken)
        {
            var from = DateOnly.FromDateTime(request.From);
            var to = DateOnly.FromDateTime(request.To);

            var entries = await _context.Entries
                .Include(e => e.Project)
                .Include(e => e.Activity)
                .AsSplitQuery()
                .Where(e => e.Project.Id == request.ProjectId)
                .Where(e => e.Date > from && e.Date < to)
                .Where(e => e.TimeSheet.Status == Domain.Entities.TimeSheetStatus.Approved)
                .ToArrayAsync(cancellationToken);

            var entriesByActivity = entries.GroupBy(x => x.Activity);

            if (!entries.Any())
            {
                return MediatR.Unit.Value;
            }

            var invoice = await _invoicesClient.CreateInvoiceAsync(new CreateInvoice()
            {
                 Date = DateTime.Now,
                 Note = entriesByActivity.First().Key.Project.Name
            });

            foreach(var entryGroup in entriesByActivity)
            {
                var description = entryGroup.Key.Name;
                var hourlyRate = entryGroup.Key.HourlyRate.GetValueOrDefault();

                var hours = entryGroup.Sum(e => e.Hours.GetValueOrDefault());

                await _invoicesClient.AddItemAsync(
                    invoice.Id,
                    new AddInvoiceItem {
                        ProductType = ProductType.Service,
                        Description = description,
                        UnitPrice = hourlyRate.GetVatFromTotal(0.25),
                        Unit = "hours",
                        VatRate = 0.25,
                        Quantity = hours
                    });
            }

            // TODO: Mark TimeSheets as billed.

            return MediatR.Unit.Value;
        }
    }
}

