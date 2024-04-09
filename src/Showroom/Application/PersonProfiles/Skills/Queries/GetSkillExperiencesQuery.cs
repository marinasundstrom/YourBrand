using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Showroom.Application.Common.Interfaces;

namespace YourBrand.Showroom.Application.PersonProfiles.Skills.Queries;

public record GetSkillExperiencesQuery(string PersonProfileId, string Id) : IRequest<PersonProfileSkillExperiencesDto?>
{
    sealed class GetSkillExperiencesQueryHandler(
        IShowroomContext context,
        IUserContext userContext) : IRequestHandler<GetSkillExperiencesQuery, PersonProfileSkillExperiencesDto?>
    {
        public async Task<PersonProfileSkillExperiencesDto?> Handle(GetSkillExperiencesQuery request, CancellationToken cancellationToken)
        {
            var personProfileSkill = await context
               .PersonProfileSkills
               .Include(x => x.Skill)
               .ThenInclude(x => x.Area)
                .ThenInclude(x => x.Industry)
               .AsNoTracking()
               .FirstAsync(c => c.PersonProfile.Id == request.PersonProfileId && c.Id == request.Id, cancellationToken);

            var personProfileExperiences = await context
               .PersonProfileExperiences
               .Where(x => x.PersonProfile.Id == request.PersonProfileId)
               .Include(x => x.Company)
                .ThenInclude(x => x.Industry)
               .Include(x => x.Skills)
               .ThenInclude(x => x.PersonProfileSkill)
               .AsNoTracking()
               .ToArrayAsync(cancellationToken);

            var experiences = new List<SkillExperienceDto>();

            foreach (var personProfileExperience in personProfileExperiences)
            {
                var hasSkill = personProfileExperience.Skills.Any(x => x.PersonProfileSkill.Id == personProfileSkill.Id);
                if (hasSkill)
                {
                    experiences.Add(new SkillExperienceDto(personProfileExperience.Id, personProfileExperience.Company.Name, personProfileExperience.Title, personProfileExperience.StartDate, personProfileExperience.EndDate));
                }
            }

            if (personProfileSkill is null)
            {
                return null;
            }

            return new PersonProfileSkillExperiencesDto(personProfileSkill.Id, personProfileSkill.Skill.ToDto(), personProfileSkill.Level, personProfileSkill.Comment, personProfileSkill.Link?.ToDto(), experiences.OrderByDescending(x => x.StartDate));
        }
    }
}