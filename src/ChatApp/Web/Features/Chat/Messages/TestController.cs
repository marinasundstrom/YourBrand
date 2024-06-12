namespace ChatApp.Features.Chat.Messages;

/*
[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/[controller]")]
public sealed class Test : ControllerBase
{
    private readonly IPublishEndpoint publishEndpoint;

    public Test(IPublishEndpoint publishEndpoint)
    {
        this.publishEndpoint = publishEndpoint;
    }

    [FeatureGate("FeatureA")]
    [HttpPut("{id}/status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesDefaultResponseType]
    public async Task UpdateStatus(int id, [FromBody] Application.Features.Todos.TodoStatusDto status, CancellationToken cancellationToken)
    {
        await publishEndpoint.Publish(new UpdateStatus(id, (Contracts.TodoStatus)status), cancellationToken);
    }
}
*/
