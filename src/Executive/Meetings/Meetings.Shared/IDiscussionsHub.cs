namespace YourBrand.Meetings;

public interface IDiscussionsHub
{
    Task RequestSpeakerTime();
    Task RevokeSpeakerTime();
}