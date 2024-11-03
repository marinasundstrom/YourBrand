﻿using YourBrand.Domain;

namespace YourBrand.Meetings.Services;

public interface IDomainEventDispatcher
{
    Task Dispatch(DomainEvent domainEvent, CancellationToken cancellationToken = default);
}