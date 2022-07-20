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
    protected readonly IDomainEventService fakeDomainEventService;
    protected readonly ICurrentUserService fakeCurrentUserService;
    protected readonly ITenantService fakeTenantService;
    protected readonly IDateTime fakeDateTimeService;
    protected readonly IApiApplicationContext fakeApiApplicationContext;

    public TestBase()
    {
        fakeDomainEventService = Substitute.For<IDomainEventService>();

        fakeCurrentUserService = Substitute.For<ICurrentUserService>();

        fakeTenantService = Substitute.For<ITenantService>();

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

        return new TimeReportContext(options, fakeTenantService, fakeDomainEventService, fakeApiApplicationContext,
            new YourBrand.TimeReport.Infrastructure.Persistence.Interceptors.AuditableEntitySaveChangesInterceptor(fakeCurrentUserService, fakeDateTimeService, fakeTenantService));
    }
}