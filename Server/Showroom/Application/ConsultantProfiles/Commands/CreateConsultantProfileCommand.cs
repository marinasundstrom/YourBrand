using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Skynet.Showroom.Application.Common.Interfaces;
using Skynet.Showroom.Domain.Entities;
using Skynet.Showroom.Domain.Exceptions;

namespace Skynet.Showroom.Application.ConsultantProfiles.Commands;

public class CreateConsultantProfileCommand : IRequest<ConsultantProfileDto>
{
    public CreateConsultantProfileCommand(AddConsultantProfileDto consultantProfile)
    {
        ConsultantProfile = consultantProfile;
    }

    public AddConsultantProfileDto ConsultantProfile { get; }

    class CreateConsultantProfileCommandHandler : IRequestHandler<CreateConsultantProfileCommand, ConsultantProfileDto>
    {
        private readonly IShowroomContext _context;
        private readonly ICurrentUserService currentUserService;

        public CreateConsultantProfileCommandHandler(
            IShowroomContext context,
            ICurrentUserService currentUserService)
        {
            _context = context;
            this.currentUserService = currentUserService;
        }

        public async Task<ConsultantProfileDto> Handle(CreateConsultantProfileCommand request, CancellationToken cancellationToken)
        {
            var consultantProfile = new ConsultantProfile
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = request.ConsultantProfile.FirstName,
                LastName = request.ConsultantProfile.LastName,
                DisplayName = request.ConsultantProfile.DisplayName,
                Headline = request.ConsultantProfile.Headline,
                ShortPresentation = "",
                Presentation = "",
                ManagerId = ""
            };

            if (consultantProfile.AvailableFromDate is not null)
            {
                consultantProfile.AvailableFromDate = consultantProfile.AvailableFromDate?.Date;
            }

            if (request.ConsultantProfile.OrganizationId is not null)
            {
                consultantProfile.OrganizationId = request.ConsultantProfile.OrganizationId;
            }

            if (request.ConsultantProfile.CompetenceAreaId is not null)
            {
                consultantProfile.CompetenceAreaId = request.ConsultantProfile.CompetenceAreaId;
            }

            _context.ConsultantProfiles.Add(consultantProfile);

            await _context.SaveChangesAsync(cancellationToken);

            return consultantProfile.ToDto(null);
        }
    }
}
