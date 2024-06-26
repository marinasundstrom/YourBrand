﻿using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Showroom.Application.Common.Interfaces;

namespace YourBrand.Showroom.Application.CompetenceAreas.Commands;

public record DeleteCompetenceAreaCommand(string Id) : IRequest
{
    public class DeleteCompetenceAreaCommandHandler(IShowroomContext context) : IRequestHandler<DeleteCompetenceAreaCommand>
    {
        public async Task Handle(DeleteCompetenceAreaCommand request, CancellationToken cancellationToken)
        {
            var comepetenceArea = await context.CompetenceAreas
                .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (comepetenceArea is null) throw new Exception();

            context.CompetenceAreas.Remove(comepetenceArea);

            await context.SaveChangesAsync(cancellationToken);

        }
    }
}