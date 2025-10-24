using System.Collections.Generic;

namespace YourBrand.Meetings;

public static class AgendaItemTypeExtensions
{
    private static readonly HashSet<AgendaItemTypeEnum> DiscussionTypes = new()
    {
        AgendaItemTypeEnum.PublicComment,
        AgendaItemTypeEnum.SpecialPresentations,
        AgendaItemTypeEnum.OldBusiness,
        AgendaItemTypeEnum.UnfinishedBusiness,
        AgendaItemTypeEnum.NewBusiness,
        AgendaItemTypeEnum.Discussion,
        AgendaItemTypeEnum.StrategicDiscussion,
        AgendaItemTypeEnum.ExecutiveSession,
        AgendaItemTypeEnum.ActionItemsReview,
        AgendaItemTypeEnum.GuestSpeakers,
        AgendaItemTypeEnum.FollowUpItems
    };

    private static readonly HashSet<AgendaItemTypeEnum> VotingTypes = new()
    {
        AgendaItemTypeEnum.ApprovalOfMinutes,
        AgendaItemTypeEnum.ApprovalOfAgenda,
        AgendaItemTypeEnum.ConsentAgenda,
        AgendaItemTypeEnum.Motion,
        AgendaItemTypeEnum.Voting,
        AgendaItemTypeEnum.Election
    };

    public static bool RequiresDiscussion(this AgendaItemTypeEnum type) => DiscussionTypes.Contains(type);

    public static bool RequiresVoting(this AgendaItemTypeEnum type) => VotingTypes.Contains(type);
}
