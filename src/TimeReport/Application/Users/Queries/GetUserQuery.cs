﻿
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;

namespace YourBrand.TimeReport.Application.Users.Queries;

public record GetUserQuery(string UserId) : IRequest<Result<UserDto>>
{
    public class GetUserQueryHandler(ITimeReportContext context) : IRequestHandler<GetUserQuery, Result<UserDto>>
    {
        public async Task<Result<UserDto>> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            var user = await context.Users
                .AsNoTracking()
                .AsSplitQuery()
                .Include(x => x.Teams)
                .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);

            if (user is null)
            {
                return null!;
            }

            return user.ToDto();
        }
    }
}