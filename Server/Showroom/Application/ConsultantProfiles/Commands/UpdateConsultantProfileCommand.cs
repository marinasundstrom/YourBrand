using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using YourCompany.Showroom.Application.Common.Interfaces;
using YourCompany.Showroom.Domain.Entities;
using YourCompany.Showroom.Domain.Exceptions;

namespace YourCompany.Showroom.Application.ConsultantProfiles.Commands;

public class UpdateConsultantProfileCommand : IRequest<ConsultantProfileDto>
{
    public UpdateConsultantProfileCommand(string id, UpdateConsultantProfileDto consultantProfile)
    {
        Id = id;
        ConsultantProfile = consultantProfile;
    }

    public string Id { get; set; }

    public UpdateConsultantProfileDto ConsultantProfile { get; }

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
