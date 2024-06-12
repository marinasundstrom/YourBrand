using FluentValidation;

using MediatR;

using YourBrand.ChatApp.Domain.Repositories;
using YourBrand.ChatApp.Domain;

namespace YourBrand.ChatApp.Features.Organizations;

public record UpdateOrganization(string OrganizationId, string Name) : IRequest<Result<OrganizationDto>>
{
    public class Validator : AbstractValidator<UpdateOrganization>
    {
        public Validator()
        {
        }
    }

    public class Handler(IOrganizationRepository organizationRepository, IUnitOfWork unitOfWork) : IRequestHandler<UpdateOrganization, Result<OrganizationDto>>
    {
        public async Task<Result<OrganizationDto>> Handle(UpdateOrganization request, CancellationToken cancellationToken)
        {
            var user = await organizationRepository.FindByIdAsync(request.OrganizationId!, cancellationToken);

            if (user is null)
            {
                return Result.Failure<OrganizationDto>(Errors.Organizations.OrganizationNotFound);
            }

            user.Name = request.Name;
            //user.Fr = request.Name;

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(user.ToDto());
        }
    }
}