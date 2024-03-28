
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;

using static YourBrand.TimeReport.Application.Projects.Expenses.ExpensesHelpers;

namespace YourBrand.TimeReport.Application.Projects.Expenses.Commands;

public record UploadExpenseAttachmentCommand(string ExpenseId, string Name, Stream Stream) : IRequest<string?>
{
    public class UploadExpenseAttachmentCommandHandler : IRequestHandler<UploadExpenseAttachmentCommand, string?>
    {
        private readonly ITimeReportContext _context;
        private readonly IBlobService _blobService;

        public UploadExpenseAttachmentCommandHandler(ITimeReportContext context, IBlobService blobService)
        {
            _context = context;
            _blobService = blobService;
        }

        public async Task<string?> Handle(UploadExpenseAttachmentCommand request, CancellationToken cancellationToken)
        {
            var expense = await _context.Expenses
                .Include(x => x.Project)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.ExpenseId, cancellationToken);

            if (expense is null)
            {
                throw new Exception();
            }

            if (!string.IsNullOrEmpty(expense.Attachment))
            {
                throw new Exception();
            }

            var blobName = $"{expense.Id}-{request.Name}";

            await _blobService.UploadBloadAsync(blobName, request.Stream);

            expense.Attachment = blobName;

            await _context.SaveChangesAsync(cancellationToken);

            return GetAttachmentUrl(expense.Attachment);
        }
    }
}