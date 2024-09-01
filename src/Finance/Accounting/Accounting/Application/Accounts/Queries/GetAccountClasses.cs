﻿using System.ComponentModel.DataAnnotations;

using MediatR;

namespace YourBrand.Accounting.Application.Accounts.Queries;

public record GetAccountClassesQuery(string OrganizationId) : IRequest<IEnumerable<AccountClassDto>>
{
    public class GetAccountClassesQueryHandler : IRequestHandler<GetAccountClassesQuery, IEnumerable<AccountClassDto>>
    {
        public Task<IEnumerable<AccountClassDto>> Handle(GetAccountClassesQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult<IEnumerable<AccountClassDto>>(Enum.GetValues<Domain.Enums.AccountClass>().Select(ac =>
            {
                return new AccountClassDto
                {
                    Id = (int)ac,
                    Description = ac.GetAttribute<DisplayAttribute>()!.Name!
                };
            }));
        }
    }
}