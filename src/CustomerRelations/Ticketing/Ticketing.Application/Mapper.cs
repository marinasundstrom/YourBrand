using YourBrand.Ticketing.Application.Features.Tickets.Dtos;
using YourBrand.Ticketing.Application.Features.Users;
using YourBrand.Ticketing.Application.Features.Organizations;
using YourBrand.Ticketing.Domain.Enums;
using YourBrand.Ticketing.Application.Features.Projects;
using YourBrand.Ticketing.Application.Features.Projects.ProjectGroups;
using YourBrand.Ticketing.Application.Features.Teams;

namespace YourBrand.Ticketing.Application;

public static partial class Mappings
{
    public static TicketDto ToDto(this Ticket ticket) => new(
        ticket.Id,
        null!,
        ticket.Subject,
        ticket.Text,
        ticket.Status.ToDto()!,
        ticket.Assignee?.ToDto(),
        ticket.LastMessage,
        ticket.Text,
        ticket.Type!.ToDto(),
        ticket.Priority?.ToDto(),
        ticket.Urgency?.ToDto(),
        ticket.Impact?.ToDto(),
        ticket.EstimatedHours,
        ticket.RemainingHours,
        ticket.Tags.Select(x => x.Tag).Select(x => x.ToDto()),
        ticket.Attachments.Select(x => x.ToDto()),
        ticket.Created, ticket.CreatedBy?.ToDto(), ticket.LastModified, ticket.LastModifiedBy?.ToDto());

    //public static TicketCommentDto ToDto(this TicketComment ticketComment) => new TicketCommentDto(ticketComment.Id, ticketComment.Text, ticketComment.Created, ticketComment.CreatedBy?.ToDto(), ticketComment.LastModified, ticketComment.LastModifiedBy?.ToDto());

    public static TicketTypeDto ToDto(this TicketType ticketType) => new TicketTypeDto(ticketType.Id, ticketType.Name);

    public static TicketPriorityDto ToDto(this TicketPriority priority) => (TicketPriorityDto)priority;

    public static TicketUrgencyDto ToDto(this TicketUrgency urgency) => (TicketUrgencyDto)urgency;

    public static TicketImpactDto ToDto(this TicketImpact impact) => (TicketImpactDto)impact;

    public static TicketStatusDto ToDto(this TicketStatus ticketStatus) => new TicketStatusDto(ticketStatus.Id, ticketStatus.Name);

    public static AttachmentDto ToDto(this Attachment attachment) => new AttachmentDto(attachment.Id, attachment.Name);

    public static TagDto ToDto(this Tag tag) => new TagDto(tag.Id, tag.Name);

    public static UserDto ToDto(this User user) => new(user.Id, user.Name);

    public static UserInfoDto ToDto2(this User user) => new(user.Id, user.Name);

    public static OrganizationDto ToDto(this Organization user) => new(user.Id, user.Name);

    public static OrganizationDto ToDto2(this Organization user) => new(user.Id, user.Name);

    public static TicketParticipantDto ToDto(this TicketParticipant participant) => new(participant.Id, participant.Name!, null);

    public static ProjectDto ToDto(this Domain.Entities.Project project)
    {
        return new(project.Id, project.Name, project.Description, new OrganizationDto("", "") /* project.Organization.ToDto()*/, project.Teams.Select(t => t.ToDto()));
    }

    public static ProjectGroupDto ToDto(this Domain.Entities.ProjectGroup projectGroup)
    {
        return new(projectGroup.Id, projectGroup.Name, projectGroup.Description, projectGroup.Project?.ToDto());
    }

    public static ProjectMembershipDto ToDto(this Domain.Entities.ProjectMembership projectMembership)
    {
        return new ProjectMembershipDto(projectMembership.Id, projectMembership.Project.ToDto(),
                projectMembership.User.ToDto(),
                projectMembership.From, projectMembership.To);
    }

    public static TeamDto ToDto(this Domain.Entities.Team team)
    {
        return new(team.Id, team.Name, team.Memberships.Select(x => x.ToDto()));
    }

    public static TeamMemberDto ToDto(this Domain.Entities.TeamMembership teamMember)
    {
        return new(teamMember.User.Id, teamMember.User.Name, "");
    }

    public static TeamMembershipDto ToDto2(this Domain.Entities.TeamMembership teamMembership)
    {
        return new(teamMembership.Id, teamMembership.User.ToDto());
    }
}