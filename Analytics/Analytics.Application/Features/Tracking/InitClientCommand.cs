using MediatR;

namespace YourBrand.Analytics.Application.Features.Tracking;

public record InitClientCommand(string UserAgent) : IRequest<string>
{
    public class Handler : IRequestHandler<InitClientCommand, string>
    {
        private readonly IApplicationDbContext context;

        public Handler(IApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<string> Handle(InitClientCommand request, CancellationToken cancellationToken)
        {
            int number = System.Security.Cryptography.RandomNumberGenerator.GetInt32(1000000, 2000000);

            string id = $"{number}.{DateTimeOffset.UtcNow.Ticks}";

            var client = new Client(id, request.UserAgent);

            context.Clients.Add(client);
            await context.SaveChangesAsync(cancellationToken);

            return id;
        }
    }
}
