using System;

using MediatR;

namespace Worker.Application;

public class DoSomethingCommand : IRequest<double>
{
    public DoSomethingCommand(double lhs, double rhs)
    {
        Lhs = lhs;
        Rhs = rhs;
    }

    public double Lhs { get; }

    public double Rhs { get; }

    class DoSomethingCommandHandler : IRequestHandler<DoSomethingCommand, double>
    {
        public async Task<double> Handle(DoSomethingCommand request, CancellationToken cancellationToken)
        {
            await Task.Delay(Random.Shared.Next(5000));

            return request.Lhs + request.Rhs;
        }
    }
}