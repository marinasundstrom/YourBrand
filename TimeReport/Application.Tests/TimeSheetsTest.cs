using System;
using System.Threading.Tasks;

using YourBrand.TimeReport.Application.Common;
using YourBrand.TimeReport.Application.TimeSheets.Queries;
using YourBrand.TimeReport.Domain;
using YourBrand.TimeReport.Domain.Events;
using YourBrand.TimeReport.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

using NSubstitute;

using Shouldly;

using Xunit;
using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Domain.Entities;
using YourBrand.TimeReport.Application.TimeSheets;

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

        */

        var commandHandler = new GetTimeSheetForWeekQuery.GetTimeSheetForWeekQueryHandler(context, fakeCurrentUserService);

        var initialItemsCount = await context.TimeSheetActivities.CountAsync();

        // Act

        var getTimeSheetForWeekQuery = new GetTimeSheetForWeekQuery(2020, 31, null);

        TimeSheetDto? item = await commandHandler.Handle(getTimeSheetForWeekQuery, default);

        // Assert

        var newItemsCount = await context.TimeSheetActivities.CountAsync();

        newItemsCount.ShouldBeGreaterThan(initialItemsCount);

        // Has Domain Event been published ?

        /*
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