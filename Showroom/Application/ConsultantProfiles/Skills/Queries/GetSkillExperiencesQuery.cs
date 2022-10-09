using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Showroom.Application.Common.Interfaces;

namespace YourBrand.Showroom.Application.ConsultantProfiles.Skills.Queries;

public record GetSkillExperiencesQuery(string ConsultantProfileId, string Id) : IRequest<ConsultantProfileSkillExperiencesDto?>
{

    class GetSkillExperiencesQueryHandler : IRequestHandler<GetSkillExperiencesQuery, ConsultantProfileSkillExperiencesDto?>
    {
        private readonly IShowroomContext _context;
        private readonly ICurrentUserService currentUserService;

        public GetSkillExperiencesQueryHandler(
            IShowroomContext context,
            ICurrentUserService currentUserService)
        {
            _context = context;
            this.currentUserService = currentUserService;
        }

        public async Task<ConsultantProfileSkillExperiencesDto?> Handle(GetSkillExperiencesQuery request, CancellationToken cancellationToken)
        {
            var consultantProfileSkill = await _context
               .ConsultantProfileSkills
               .Include(x => x.Skill)
               .ThenInclude(x => x.Area)
                .ThenInclude(x => x.Industry)
               .AsNoTracking()
               .FirstAsync(c => c.ConsultantProfile.Id == request.ConsultantProfileId && c.Id == request.Id, cancellationToken);

            var consultantProfileExperiences = await _context
               .ConsultantProfileExperiences
               .Where(x => x.ConsultantProfile.Id == request.ConsultantProfileId)
               .Include(x => x.Company)
                .ThenInclude(x => x.Industry)
               .Include(x => x.Skills)
               .ThenInclude(x => x.ConsultantProfileSkill)
               .AsNoTracking()
               .ToArrayAsync(cancellationToken);

               var experiences = new List<SkillExperienceDto>(); 

               foreach(var consultantProfileExperience in consultantProfileExperiences) 
               {
                    var hasSkill = consultantProfileExperience.Skills.Any(x => x.ConsultantProfileSkill.Id == consultantProfileSkill.Id);
                    if(hasSkill) 
                    {
                        experiences.Add(new SkillExperienceDto(consultantProfileExperience.Id, consultantProfileExperience.Company.Name, consultantProfileExperience.Title, consultantProfileExperience.StartDate, consultantProfileExperience.EndDate));
                    }
               }

            if (consultantProfileSkill is null)
            {
                return null;
            }

            return new ConsultantProfileSkillExperiencesDto(consultantProfileSkill.Id, consultantProfileSkill.Skill.ToDto(), consultantProfileSkill.Level, consultantProfileSkill.Comment, consultantProfileSkill.Link?.ToDto(), experiences.OrderByDescending(x => x.StartDate));
        }
    }
}
