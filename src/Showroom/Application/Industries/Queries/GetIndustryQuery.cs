using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Showroom.Application.Common.Interfaces;

namespace YourBrand.Showroom.Application.Industries.Queries;

public record GetIndustryQuery(int Id) : IRequest<IndustryDto?>
{
    sealed class GetIndustryQueryHandler(
        IShowroomContext context,
        IUserContext userContext) : IRequestHandler<GetIndustryQuery, IndustryDto?>
    {
        public async Task<IndustryDto?> Handle(GetIndustryQuery request, CancellationToken cancellationToken)
        {
            var industry = await context
               .Industries
               .AsNoTracking()
               .FirstAsync(c => c.Id == request.Id);

            if (industry is null)
            {
                return null;
            }

            return industry.ToDto();
        }
    }
}