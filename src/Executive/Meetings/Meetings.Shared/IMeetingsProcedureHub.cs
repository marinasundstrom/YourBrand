namespace YourBrand.Meetings;

public interface IMeetingsProcedureHub
{
    Task ChangeAgendaItem(string agendaItemId);
}