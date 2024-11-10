namespace YourBrand.Meetings;

public interface IMeetingsProcedureHub : IDiscussionsHub, IVotingHub, IElectionsHub
{
    Task ChangeAgendaItem(string agendaItemId);
}