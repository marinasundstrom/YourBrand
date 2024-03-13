using MediatR;

namespace YourBrand.Analytics.Application.Features.Tracking;

public record InitSessionCommand(string ClientId, string? IPAddress) : IRequest<string>
{
    public class Handler : IRequestHandler<InitSessionCommand, string>
    {
        private readonly IApplicationDbContext context;

        public Handler(IApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<string> Handle(InitSessionCommand request, CancellationToken cancellationToken)
        {
            var session = new Session(request.ClientId, request.IPAddress, DateTimeOffset.UtcNow);

            context.Sessions.Add(session);
            await context.SaveChangesAsync(cancellationToken);

            return session.Id;
        }
    }
}
