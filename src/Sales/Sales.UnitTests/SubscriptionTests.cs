using System;

using FluentAssertions;

using NSubstitute;

using Xunit;

using YourBrand.Sales.Domain.Entities;
using YourBrand.Sales.Domain.Enums;
using YourBrand.Sales.Features.SubscriptionManagement;

namespace YourBrand.Sales.UnitTests;


public class SubscriptionTests
{
    [Fact]
    public void StartTrial_ShouldSetTrialDatesAndStatus()
    {
        // Arrange
        var subscription = new Subscription();
        var timeProvider = Substitute.For<TimeProvider>();
        timeProvider.GetUtcNow().Returns(DateTimeOffset.UtcNow);
        int trialDays = 10;

        // Act
        subscription.StartTrial(trialDays, timeProvider);

        // Assert
        subscription.TrialStartDate.Should().Be(timeProvider.GetUtcNow());
        subscription.TrialEndDate.Should().Be(timeProvider.GetUtcNow().AddDays(trialDays));
        subscription.StatusId.Should().Be((int)SubscriptionStatusEnum.Trial);
    }

    [Fact]
    public void EndTrial_WithSuccessfulPayment_ShouldActivateSubscription()
    {
        // Arrange
        var subscription = new Subscription
        {
            StatusId = (int)SubscriptionStatusEnum.Trial,
            TrialStartDate = DateTimeOffset.UtcNow.AddDays(-5),
            TrialEndDate = DateTimeOffset.UtcNow.AddDays(5),
            PaymentStatus = PaymentStatus.PaymentSucceeded
        };
        var timeProvider = Substitute.For<TimeProvider>();
        timeProvider.GetUtcNow().Returns(DateTimeOffset.UtcNow);

        // Act
        subscription.EndTrial(timeProvider);

        // Assert
        subscription.StatusId.Should().Be((int)SubscriptionStatusEnum.Active);
        subscription.HasUsedTrial.Should().BeTrue();
    }

    [Fact]
    public void EndTrial_WithFailedPayment_ShouldSuspendSubscription()
    {
        // Arrange
        var subscription = new Subscription
        {
            StatusId = (int)SubscriptionStatusEnum.Trial,
            TrialStartDate = DateTimeOffset.UtcNow.AddDays(-5),
            TrialEndDate = DateTimeOffset.UtcNow.AddDays(5),
            PaymentStatus = PaymentStatus.PaymentFailed
        };
        var timeProvider = Substitute.For<TimeProvider>();
        timeProvider.GetUtcNow().Returns(DateTimeOffset.UtcNow);

        // Act
        subscription.EndTrial(timeProvider);

        // Assert
        subscription.StatusId.Should().Be((int)SubscriptionStatusEnum.Suspended);
        subscription.HasUsedTrial.Should().BeTrue();
    }

    [Fact]
    public void Activate_WithTrial_ShouldEndTrialAndActivate()
    {
        // Arrange
        var subscription = new Subscription
        {
            TrialStartDate = DateTimeOffset.UtcNow.AddDays(-5),
            TrialEndDate = DateTimeOffset.UtcNow.AddDays(5),
            PaymentStatus = PaymentStatus.PaymentSucceeded,
            StatusId = (int)SubscriptionStatusEnum.Trial
        };
        var timeProvider = Substitute.For<TimeProvider>();
        timeProvider.GetUtcNow().Returns(DateTimeOffset.UtcNow);

        // Act
        subscription.Activate(timeProvider);

        // Assert
        subscription.StatusId.Should().Be((int)SubscriptionStatusEnum.Active);
    }

    [Fact]
    public void Activate_ShouldThrow_WhenSubscriptionIsCanceled()
    {
        // Arrange
        var subscription = new Subscription
        {
            StatusId = (int)SubscriptionStatusEnum.Canceled
        };
        var timeProvider = Substitute.For<TimeProvider>();

        // Act
        Action act = () => subscription.Activate(timeProvider);

        // Assert
        act.Should().Throw<InvalidOperationException>().WithMessage("Cannot activate a subscription that is canceled, expired, or suspended.");
    }

    [Fact]
    public void Pause_ShouldUpdateStatus_WhenSubscriptionIsActive()
    {
        // Arrange
        var subscription = new Subscription
        {
            StatusId = (int)SubscriptionStatusEnum.Active
        };
        var timeProvider = Substitute.For<TimeProvider>();

        // Act
        subscription.Pause(timeProvider);

        // Assert
        subscription.StatusId.Should().Be((int)SubscriptionStatusEnum.Paused);
    }

    [Fact]
    public void Pause_ShouldThrow_WhenSubscriptionIsNotActive()
    {
        // Arrange
        var subscription = new Subscription
        {
            StatusId = (int)SubscriptionStatusEnum.Canceled
        };
        var timeProvider = Substitute.For<TimeProvider>();

        // Act
        Action act = () => subscription.Pause(timeProvider);

        // Assert
        act.Should().Throw<InvalidOperationException>().WithMessage("Can only pause an active subscription.");
    }

    [Fact]
    public void RequestCancellation_ShouldUpdateCancellationStatusAndDate()
    {
        // Arrange
        var subscription = new Subscription
        {
            StatusId = (int)SubscriptionStatusEnum.Active
        };
        var timeProvider = Substitute.For<TimeProvider>();
        timeProvider.GetUtcNow().Returns(DateTimeOffset.UtcNow);

        // Act
        subscription.RequestCancellation(timeProvider);

        // Assert
        subscription.CancellationStatus.Should().Be(CancellationStatus.CancellationRequested);
        subscription.CancellationRequestedDate.Should().Be(timeProvider.GetUtcNow());
    }

    [Fact]
    public void FinalizeCancellation_ShouldUpdateStatus_WhenFinalizationPeriodPassed()
    {
        // Arrange
        var subscription = new Subscription
        {
            StatusId = (int)SubscriptionStatusEnum.Active,
            CancellationStatus = CancellationStatus.CancellationRequested,
            CancellationRequestedDate = DateTimeOffset.UtcNow.AddDays(-5),
            CancellationFinalizationPeriod = TimeSpan.FromDays(3)
        };
        var timeProvider = Substitute.For<TimeProvider>();
        timeProvider.GetUtcNow().Returns(DateTimeOffset.UtcNow);

        // Act
        subscription.FinalizeCancellation(timeProvider);

        // Assert
        subscription.StatusId.Should().Be((int)SubscriptionStatusEnum.Canceled);
        subscription.CancellationStatus.Should().Be(CancellationStatus.Canceled);
    }

    [Fact]
    public void SetPaymentStatus_ShouldSuspendSubscription_WhenPaymentFailed()
    {
        // Arrange
        var subscription = new Subscription
        {
            StatusId = (int)SubscriptionStatusEnum.Active
        };
        var timeProvider = Substitute.For<TimeProvider>();
        var billingDateCalculator = Substitute.For<IBillingDateCalculator>();
        var deliveryDateCalculator = Substitute.For<IDeliveryDateCalculator>();

        // Act
        subscription.SetPaymentStatus(PaymentStatus.PaymentFailed, billingDateCalculator, deliveryDateCalculator, timeProvider);

        // Assert
        subscription.StatusId.Should().Be((int)SubscriptionStatusEnum.Suspended);
    }
}