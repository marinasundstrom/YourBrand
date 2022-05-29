using System;
using System.Threading.Tasks;

using YourBrand.TimeReport.Application.Common;
using YourBrand.TimeReport.Domain;
using YourBrand.TimeReport.Domain.Events;
using YourBrand.TimeReport.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

using NSubstitute;

using Shouldly;

using Xunit;
using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Domain.Entities;

namespace Tests;

public class TimeSheetsTest : TestBase
{
    [Fact]
    public async Task CreateItem_ItemCreated()
    {
        // Arrange

        User user = CreateTestUser();

        fakeCurrentUserService.UserId.Returns(x => user.Id);

        using ITimeReportContext context = CreateDbContext();

        context.Users.Add(user);

        /*

        context.Statuses.Add(new Status()
        {
            Id = 1,
            Name = "Created"
        });

        var commandHandler = new CreateItemCommand.Handler(context, fakeUrlHelper);

        var initialHandoverCount = await context.Items.CountAsync();

        // Act

        var createItemCommand = new CreateItemCommand("Test", "Blah", 1);

        ItemDto item = await commandHandler.Handle(createItemCommand, default);

        // Assert

        var newHandoverCount = await context.Items.CountAsync();

        newHandoverCount.ShouldBeGreaterThan(initialHandoverCount);

        // Has Domain Event been published ?

        await fakeDomainEventService
            .Received(1)
            .Publish(Arg.Is<ItemCreatedEvent>(d => d.ItemId == item.Id));

        */
    }

    private static User CreateTestUser()
    {
        return new YourBrand.TimeReport.Domain.Entities.User
        {
            Id = Guid.NewGuid().ToString(),
            FirstName = "Test",
            LastName = "Testsson",
            SSN = "dfsdf",
            Email = "test@email.com"
        };
    }
}