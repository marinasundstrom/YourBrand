using MediatR;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using YourBrand.IdentityService.Application.Common.Interfaces;
using YourBrand.HumanResources.Contracts;
using YourBrand.IdentityService.Domain.Entities;
using YourBrand.IdentityService.Domain.Exceptions;
using YourBrand.IdentityService.Contracts;
using YourBrand.Identity;

namespace YourBrand.IdentityService.Application.Users.Commands;

public record DeleteUserCommand(string UserId) : IRequest
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
    {
        private readonly UserManager<Person> _userManager;
        private readonly ICurrentUserService _currentUserService;
        private readonly IEventPublisher _eventPublisher;

        public DeleteUserCommandHandler(UserManager<Person> userManager, ICurrentUserService currentUserService, IEventPublisher eventPublisher)
        {
            _userManager = userManager;
            _currentUserService = currentUserService;
            _eventPublisher = eventPublisher;
        }

        public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);

            if (user is null)
            {
                throw new UserNotFoundException(request.UserId);
            }

            await _userManager.DeleteAsync(user);

            await _eventPublisher.PublishEvent(new UserDeleted(user.Id, _currentUserService.UserId));

        }
    }
}