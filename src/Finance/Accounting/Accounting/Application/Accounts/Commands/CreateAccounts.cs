using MediatR;
using YourBrand.Accounting.Application.Common.Interfaces;

namespace YourBrand.Accounting.Application.Accounts.Commands;

public record CreateAccountsCommand(string OrganizationId) : IRequest
{
    public class CreateAccountsCommandHandler(IAccountingContext context) : IRequestHandler<CreateAccountsCommand>
    {
        public async Task Handle(CreateAccountsCommand request, CancellationToken cancellationToken)
        {
            context.Accounts.AddRange(YourBrand.Accounting.Domain.Entities.Accounts.GetAll(request.OrganizationId));

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}