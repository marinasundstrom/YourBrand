
using YourBrand.Application.Common.Interfaces;
using YourBrand.Application.Common.Models;
using YourBrand.Domain;
using YourBrand.Identity;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Application.Modules;

public record GetModulesQuery() : IRequest<IEnumerable<ModuleDto>>
{
    public class Handler : IRequestHandler<GetModulesQuery, IEnumerable<ModuleDto>>
    {
        private readonly ICurrentUserService _currentUserService;

        public Handler(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        public async Task<IEnumerable<ModuleDto>> Handle(GetModulesQuery request, CancellationToken cancellationToken)
        {
            return System.Text.Json.JsonSerializer.Deserialize<IEnumerable<ModuleDto>>(
                await File.ReadAllTextAsync("modules.json", cancellationToken)
            )!;
        }
    }
}