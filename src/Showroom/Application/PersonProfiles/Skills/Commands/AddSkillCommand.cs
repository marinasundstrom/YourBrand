using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Showroom.Application.Common.Interfaces;
using YourBrand.Showroom.Application.PersonProfiles.Skills.Queries;
using YourBrand.Showroom.Domain.Enums;

namespace YourBrand.Showroom.Application.PersonProfiles.Skills.Commands;

public record AddSkillCommand(string PersonProfileId, string SkillId, SkillLevel Level, string? Comment) : IRequest<PersonProfileSkillDto>
{
    public class AddSkillCommandHandler(IShowroomContext context) : IRequestHandler<AddSkillCommand, PersonProfileSkillDto>
    {
        public async Task<PersonProfileSkillDto> Handle(AddSkillCommand request, CancellationToken cancellationToken)
        {
            var personProfile = await context.PersonProfiles
                .FirstOrDefaultAsync(i => i.Id == request.PersonProfileId, cancellationToken);

            if (personProfile is null) throw new Exception();

            var skill = await context.Skills
                .FirstOrDefaultAsync(i => i.Id == request.SkillId, cancellationToken);

            if (skill is null) throw new Exception();

            var personProfileSkill = await context.PersonProfileSkills
                .Where(x => x.PersonProfileId == request.PersonProfileId)
                .Include(x => x.Skill)
                .FirstOrDefaultAsync(i => i.Skill.Id == request.SkillId, cancellationToken);

            if (personProfileSkill is not null) throw new Exception();

            personProfileSkill = new Domain.Entities.PersonProfileSkill()
            {
                PersonProfileId = personProfile.Id,
                Skill = skill,
                Level = request.Level,
                Comment = request.Comment
            };

            context.PersonProfileSkills.Add(personProfileSkill);

            await context.SaveChangesAsync(cancellationToken);

            personProfileSkill = await context
               .PersonProfileSkills
               .Include(x => x.Skill)
               .ThenInclude(x => x.Area)
               .ThenInclude(x => x.Industry)
               .AsNoTracking()
               .FirstAsync(c => c.SkillId == request.SkillId, cancellationToken);

            return personProfileSkill.ToDto();
        }
    }
}