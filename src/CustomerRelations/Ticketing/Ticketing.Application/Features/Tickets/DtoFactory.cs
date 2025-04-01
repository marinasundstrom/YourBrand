using YourBrand.Identity;
using YourBrand.Ticketing.Application.Features.Organizations;
using YourBrand.Ticketing.Application.Features.Projects;
using YourBrand.Ticketing.Application.Features.Projects.ProjectGroups;
using YourBrand.Ticketing.Application.Features.Teams;
using YourBrand.Ticketing.Application.Features.Tickets.Dtos;
using YourBrand.Ticketing.Application.Features.Users;
using YourBrand.Ticketing.Domain.ValueObjects;

namespace YourBrand.Ticketing.Application;

public interface IDtoFactory
{
    TicketDto CreateTicketDto(Ticket ticket, Project project, TicketParticipant? assignee, TicketParticipant createdBy, TicketParticipant? editedBy, TicketParticipant? deletedBy, Dictionary<TicketParticipantId, User> users);
    TicketParticipantDto CreateParticipantDto(TicketParticipant participant, Dictionary<TicketParticipantId, User> users);
    UserDto CreateUserDto(User user);
    TicketCommentDto CreateTicketCommentDto(TicketComment ticketComment, TicketParticipant addedBy, TicketParticipant editedBy, object value, Dictionary<TicketParticipantId, User> users);
    ProjectDto CreateProjectDto(Project project);
}

public sealed class DtoFactory : IDtoFactory
{
    public TicketDto CreateTicketDto(Ticket ticket, Project project, TicketParticipant? assignee, TicketParticipant createdBy, TicketParticipant? editedBy, TicketParticipant? deletedBy, Dictionary<TicketParticipantId, User> users)
    {
        return new TicketDto(
            ticket.Id,
            project.ToDto(),
            ticket.Subject,
            ticket.Text,
            ticket.Status.ToDto()!,
            ticket.AssigneeId is null ? null : CreateParticipantDto(assignee!, users),
            ticket.LastMessage,
            ticket.Text,
            ticket.Type!.ToDto(),

            ticket.Priority?.ToDto(),
            ticket.Urgency?.ToDto(),
            ticket.Impact?.ToDto(),

            ticket.EstimatedTime,
            ticket.CompletedTime,
            ticket.RemainingTime,

            ticket.PlannedStartDate,
            ticket.StartDeadline,
            ticket.ExpectedEndDate,
            ticket.DueDate,
            ticket.ActualStartDate,
            ticket.ActualEndDate,

            ticket.Tags.Select(x => x.Tag).Select(x => x.ToDto()),
            ticket.Attachments.Select(x => x.ToDto()),
            
            ticket.Created,
            CreateParticipantDto(createdBy, users),
            ticket.LastModified,
            ticket.LastModifiedById is null ? null : CreateParticipantDto(editedBy!, users));
    }

    public UserDto CreateUserDto(User user)
    {
        return new UserDto(user!.Id.ToString(), user.Name);
    }

    public TicketParticipantDto CreateParticipantDto(TicketParticipant participant, Dictionary<TicketParticipantId, User> users)
    {
        return new TicketParticipantDto(
            participant!.Id,
            /* participant.DisplayName ?? */ users[participant.Id].Name,
            participant.UserId);
    }

    public TicketCommentDto CreateTicketCommentDto(TicketComment ticketComment, TicketParticipant addedBy, TicketParticipant editedBy, object value, Dictionary<TicketParticipantId, User> users)
    {
        return new TicketCommentDto(
         ticketComment.Id,
         ticketComment.Text,
         ticketComment.Created,
         ticketComment.CreatedById is null ? null : CreateParticipantDto(addedBy!, users),
         ticketComment.LastModified,
         ticketComment.LastModifiedById is null ? null : CreateParticipantDto(editedBy!, users));
    }

    public ProjectDto CreateProjectDto(Project project)
    {
        return new(project.Id, project.Name, project.Description, new OrganizationDto("", ""), []);
    }

    /*

public ProjectGroupDto ToDto(this Domain.Entities.ProjectGroup projectGroup)
{
    return new(projectGroup.Id, projectGroup.Name, projectGroup.Description, projectGroup.Project?.ToDto());
}

public ProjectMembershipDto ToDto(this Domain.Entities.ProjectMembership projectMembership)
{
    return new ProjectMembershipDto(projectMembership.Id, projectMembership.Project.ToDto(),
            projectMembership.User.ToDto(),
            projectMembership.From, projectMembership.To);
}

public TeamDto ToDto(this Domain.Entities.Team team)
{
    return new(team.Id, team.Name, team.Memberships.Select(x => x.ToDto()));
}

public TeamMemberDto ToDto(this Domain.Entities.TeamMembership teamMember, Dictionary<UserId, User> users)
{
    return new(teamMember.User.Id, teamMember.User.FirstName, teamMember.User.LastName);
}

public TeamMembershipDto ToDto2(this Domain.Entities.TeamMembership teamMembership)
{
    return new(teamMembership.Id, teamMembership.User.ToDto());
}

*/
}