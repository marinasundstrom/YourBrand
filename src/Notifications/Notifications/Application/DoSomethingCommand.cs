﻿using MediatR;

namespace YourBrand.Notifications.Application;

public record DoSomethingCommand(double Lhs, double Rhs) : IRequest<double>
{
    sealed class DoSomethingCommandHandler : IRequestHandler<DoSomethingCommand, double>
    {
        public async Task<double> Handle(DoSomethingCommand request, CancellationToken cancellationToken)
        {
            await Task.Delay(Random.Shared.Next(5000));

            return request.Lhs + request.Rhs;
        }
    }
}