﻿using YourBrand.Domain;

namespace YourBrand.Analytics.Application.Services;

public interface IDomainEventDispatcher
{
    Task Dispatch(DomainEvent domainEvent, CancellationToken cancellationToken = default);
}