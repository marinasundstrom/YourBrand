
using MediatR;

using Microsoft.EntityFrameworkCore;

using TimeReport.Application.Common.Interfaces;
using TimeReport.Application.Projects;

using static TimeReport.Application.Expenses.ExpensesHelpers;

namespace TimeReport.Application.Expenses.Commands;

public class UploadExpenseAttachmentCommand : IRequest<string?>
{
    public UploadExpenseAttachmentCommand(string expenseId, string name, Stream stream)
    {
        ExpenseId = expenseId;
        Name = name;
        Stream = stream;
    }

    public string ExpenseId { get; }

    public string Name { get; }

    public Stream Stream { get; }

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