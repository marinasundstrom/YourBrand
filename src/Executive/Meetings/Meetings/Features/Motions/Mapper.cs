using YourBrand.Meetings.Features.Users;
using YourBrand.Meetings.Features.Organizations;

namespace YourBrand.Meetings.Features.Motions;

public static partial class Mappings
{
    public static MotionDto ToDto(this Motion motion) => new(motion.Id, motion.Title, motion.Status, motion.Text, motion.OperativeClauses.Select(x => x.ToDto()));

    public static MotionOperativeClauseDto ToDto(this MotionOperativeClause clause) => new(clause.Id, clause.Order , clause.Action, clause.Text);
}