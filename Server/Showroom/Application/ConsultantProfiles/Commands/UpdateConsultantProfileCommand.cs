using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Skynet.Showroom.Application.Common.Interfaces;
using Skynet.Showroom.Domain.Entities;
using Skynet.Showroom.Domain.Exceptions;

namespace Skynet.Showroom.Application.ConsultantProfiles.Commands;

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

        public UpdateConsultantProfileCommandHandler(
            IShowroomContext context,
            ICurrentUserService currentUserService)
        {
            _context = context;
            this.currentUserService = currentUserService;
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

            return consultantProfile.ToDto(null);
        }
    }
}
