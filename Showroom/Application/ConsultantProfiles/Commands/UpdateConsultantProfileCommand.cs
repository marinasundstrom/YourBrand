using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using YourBrand.Showroom.Application.Common.Interfaces;
using YourBrand.Showroom.Domain.Entities;
using YourBrand.Showroom.Domain.Exceptions;

namespace YourBrand.Showroom.Application.ConsultantProfiles.Commands;

public record UpdateConsultantProfileCommand(string Id, UpdateConsultantProfileDto ConsultantProfile) : IRequest<ConsultantProfileDto>
{
    class UpdateConsultantProfileCommandHandler : IRequestHandler<UpdateConsultantProfileCommand, ConsultantProfileDto>
    {
        private readonly IShowroomContext _context;
        private readonly ICurrentUserService currentUserService;
        private readonly IUrlHelper _urlHelper;

        public UpdateConsultantProfileCommandHandler(
            IShowroomContext context,
            ICurrentUserService currentUserService,
            IUrlHelper urlHelper)
        {
            _context = context;
            this.currentUserService = currentUserService;
            _urlHelper = urlHelper;
        }

        public async Task<ConsultantProfileDto> Handle(UpdateConsultantProfileCommand request, CancellationToken cancellationToken)
        {
            var consultantProfile = await _context.ConsultantProfiles.FindAsync(request.Id);
            if (consultantProfile is null)
            {
                return null;
            }

            consultantProfile.FirstName = request.ConsultantProfile.FirstName;
            consultantProfile.LastName = request.ConsultantProfile.LastName;
            consultantProfile.DisplayName = request.ConsultantProfile.DisplayName;
            consultantProfile.OrganizationId = request.ConsultantProfile.OrganizationId;
            consultantProfile.CompetenceAreaId = request.ConsultantProfile.CompetenceAreaId;
            consultantProfile.ManagerId = "";
            consultantProfile.ShortPresentation = "";
            consultantProfile.Presentation = "";

            if (consultantProfile.AvailableFromDate is not null)
            {
                consultantProfile.AvailableFromDate = consultantProfile.AvailableFromDate?.Date;
            }

            await _context.SaveChangesAsync(cancellationToken);

            return consultantProfile.ToDto(_urlHelper);
        }
    }
}
