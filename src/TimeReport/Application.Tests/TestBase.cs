using System;

using Microsoft.EntityFrameworkCore;

using NSubstitute;

using YourBrand.ApiKeys;
using YourBrand.Identity;
using YourBrand.Tenancy;
using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Infrastructure.Persistence;

namespace Tests;

public class TestBase
{
    protected readonly IDomainEventDispatcher fakeDomainEventDispatcher;
    protected readonly IUserContext fakeUserContext;
    protected readonly ITenantContext fakeTenantContext;
    protected readonly IDateTime fakeDateTimeService;
    protected readonly IApiApplicationContext fakeApiApplicationContext;

    public TestBase()
    {
        fakeDomainEventDispatcher = Substitute.For<IDomainEventDispatcher>();

        fakeUserContext = Substitute.For<IUserContext>();

        fakeTenantContext = Substitute.For<ITenantContext>();

        fakeDateTimeService = Substitute.For<IDateTime>();
        fakeDateTimeService.Now.Returns(x => DateTime.Now);

        fakeApiApplicationContext = Substitute.For<IApiApplicationContext>();
        fakeApiApplicationContext.AppId.Returns(x => "Test");
    }

    protected ITimeReportContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<TimeReportContext>()
           .UseInMemoryDatabase(databaseName: "Test")
           .Options;

        return null!; //new TimeReportContext(options, fakeTenantContext, fakeDomainEventDispatcher, fakeApiApplicationContext,
                      //new YourBrand.TimeReport.Infrastructure.Persistence.Interceptors.AuditableEntitySaveChangesInterceptor(fakeUserContext, fakeDateTimeService, fakeTenantContext));
    }
}