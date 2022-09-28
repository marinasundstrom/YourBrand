using System;
using System.Threading.Tasks;
using YourBrand.TimeReport.Application.TimeSheets.Queries;

using Microsoft.EntityFrameworkCore;

using NSubstitute;

using Shouldly;

using Xunit;
using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Domain.Entities;
using YourBrand.TimeReport.Application.TimeSheets;
using YourBrand.TimeReport.Domain.Repositories;
using YourBrand.TimeReport.Domain;

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

        fakeCurrentUserService.UserId.Returns(x => user.Id);

        context.Users.Add(user);

        await context.SaveChangesAsync();

        /*

        context.Statuses.Add(new Status()
        {
            Id = 1,
            Name = "Created"
        });

        */

        var timeSheetRepository = Substitute.For<ITimeSheetRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var commandHandler = new GetTimeSheetForWeekQuery.GetTimeSheetForWeekQueryHandler(timeSheetRepository, unitOfWork, context, fakeCurrentUserService);

        var initialTimeSheetsCount = await context.TimeSheets.CountAsync();

        // Act

        var getTimeSheetForWeekQuery = new GetTimeSheetForWeekQuery(2020, 31, null);

        TimeSheetDto? item = await commandHandler.Handle(getTimeSheetForWeekQuery, default);

        // Assert

        var newTimeSheetsCount = await context.TimeSheets.CountAsync();

        newTimeSheetsCount.ShouldBeGreaterThan(initialTimeSheetsCount);

        // Has Domain Event been published ?

        /*
        await fakeDomainEventDispatcher
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