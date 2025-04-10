﻿using System;
using System.ComponentModel.DataAnnotations.Schema;

using Humanizer;

using YourBrand.Auditability;
using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Sales.Domain.Enums;
using YourBrand.Sales.Domain.ValueObjects;

namespace YourBrand.Sales.Domain.Entities;

public class Subscription : AggregateRoot<Guid>, IAuditableEntity<Guid, User>, ISoftDeletableWithAudit<User>, ISubscriptionParameters, IHasTenant, IHasOrganization
{
    public Subscription() : base(Guid.NewGuid())
    {
        TypeId = 1;
        StatusId = (int)SubscriptionStatusEnum.Pending;

        CancellationStatus = CancellationStatus.None;
        PaymentStatus = PaymentStatus.None;
        RenewalStatus = RenewalStatus.None;
    }

    public TenantId TenantId { get; set; }

    public OrganizationId OrganizationId { get; set; }

    public int SubscriptionNo { get; set; }

    public SubscriptionType Type { get; set; } = null!;
    public int TypeId { get; set; }

    public int? CustomerId { get; set; }

    public SubscriptionStatus Status { get; set; } = null!;
    public int StatusId { get; set; }
    public DateTimeOffset StatusDate { get; set; }

    public bool UpdateStatus(SubscriptionStatusEnum statusId, TimeProvider timeProvider)
    {
        return UpdateStatus((int)statusId, timeProvider);
    }

    public bool UpdateStatus(int statusId, TimeProvider timeProvider)
    {
        if (StatusId != statusId)
        {
            StatusId = statusId;
            StatusDate = timeProvider.GetUtcNow();
            return true;
        }

        return false;
    }

    public string? Note { get; set; }

    public Order? Order { get; set; }
    public string? OrderId { get; set; }

    public OrderItem? OrderItem { get; set; }
    public string? OrderItemId { get; set; }

    public List<Order> Orders { get; } = new List<Order>();
    public List<OrderItem> OrderItems { get; } = new List<OrderItem>();

    public SubscriptionPlan? Plan { get; set; }
    public Guid? PlanId { get; set; }

    public DateOnly StartDate { get; set; } // The start date of the subscription
    public DateOnly? EndDate { get; set; } // Nullable, only set when the subscription ends

    public PaymentStatus PaymentStatus { get; set; } // Current payment status

    public RenewalStatus RenewalStatus { get; set; } // Current renewal status
    public RenewalOption RenewalOption { get; set; }
    //public TimeInterval RenewalFrequency { get; set; } = TimeInterval.Yearly;
    //public int RenewalPeriod { get; set; } = 12; // 12 months for yearly renewals, for example
    public DateTimeOffset? NextRenewalDate { get; set; }
    public DateTimeOffset? LastRenewalDate { get; set; }

    public bool HasManualRenewal => RenewalOption == RenewalOption.Manual;
    public bool HasAutoRenewal => RenewalOption == RenewalOption.Automatic;

    public CancellationStatus CancellationStatus { get; set; } // Current cancellation status    

    // The date when the subscription was canceled (nullable if not canceled)
    public DateTimeOffset? CancellationDate { get; set; }

    public TimeSpan? CancellationFinalizationPeriod { get; set; }
    public DateTimeOffset? CancellationRequestedDate { get; set; }

    // Optional reason code or message explaining why the subscription was canceled
    public string? CancellationReason { get; set; }

    // Optional information about who or what triggered the cancellation (e.g., customer, system)
    public string? CancellationInitiator { get; set; }

    public void StartTrial(int trialDays, TimeProvider timeProvider)
    {
        if (trialDays <= 0)
        {
            throw new ArgumentException("Trial duration must be greater than zero.");
        }

        if (HasUsedTrial)
        {
            throw new InvalidOperationException("Trial has already been used.");
        }

        // Ensure the subscription is in a state where a trial can be started
        if (IsActive || IsTrial || IsCanceled || HasExpired || IsSuspended)
        {
            throw new InvalidOperationException("Cannot start a trial when the subscription is active, already in a trial, canceled, expired, or suspended.");
        }

        TrialStartDate = timeProvider.GetUtcNow();
        TrialEndDate = TrialStartDate.Value.AddDays(trialDays);
        UpdateStatus(SubscriptionStatusEnum.Trial, timeProvider);
    }

    public bool IsTrialAboutToEnd(TimeProvider timeProvider, int daysBeforeEnd = 3)
    {
        if (TrialEndDate.HasValue)
        {
            var thresholdDate = TrialEndDate.Value.AddDays(-daysBeforeEnd);
            return timeProvider.GetUtcNow() >= thresholdDate && IsTrial;
        }
        return false;
    }

    public void EndTrial(TimeProvider timeProvider)
    {
        if (!IsTrial)
        {
            throw new InvalidOperationException("Cannot end a trial when the subscription is not in a trial state.");
        }

        if (!TrialStartDate.HasValue || !TrialEndDate.HasValue)
        {
            throw new InvalidOperationException("Trial period has not been set.");
        }

        // End the trial period
        TrialEndedDate = timeProvider.GetUtcNow();

        // Set initial renewal date to the end of the trial period
        NextRenewalDate = LastRenewalDate.HasValue
            ? CalculateNextRenewalDate(LastRenewalDate.Value, Plan)
            : null;

        // Transition to the next state based on payment status
        if (HasPaymentSucceeded)
        {
            // Transition to active status if payment was successful
            SetStatusActive(timeProvider);
        }
        else
        {
            // Determine what happens when payment fails during trial end
            if (ShouldCancelOnTrialEnd())
            {
                // Cancel if the business rules state that a failed payment should cancel the subscription
                Cancel("Payment failed during trial", "System", timeProvider);
            }
            else
            {
                // Suspend the subscription if payment fails but we don't want to cancel it immediately
                SetStatusSuspended(timeProvider);
            }
        }

        // Mark that the trial has been used
        HasUsedTrial = true;
    }

    public bool ShouldCancelOnTrialEnd()
    {
        // Business logic to determine whether to cancel when a trial ends and payment fails.
        // This can be a simple flag or a more complex condition.
        return false; // For now, assuming cancellation is not immediate upon trial end
    }

    public void Activate(TimeProvider timeProvider)
    {
        // Check if the subscription is eligible to be activated
        if (IsCanceled || HasExpired || IsSuspended)
        {
            throw new InvalidOperationException("Cannot activate a subscription that is canceled, expired, or suspended.");
        }

        // Check if currently in trial
        if (IsTrial)
        {
            EndTrial(timeProvider);
        }
        else if (!IsActive)
        {
            SetStatusActive(timeProvider);

            // Set NextRenewalDate on activation based on renewal period
            if (NextRenewalDate == null)
            {
                // Calculate the next renewal date based on the renewal frequency and period
                NextRenewalDate = LastRenewalDate.HasValue
                    ? CalculateNextRenewalDate(LastRenewalDate.Value, Plan)
                    : null;
            }
        }
    }

    public void Pause(TimeProvider timeProvider)
    {
        // Check if the subscription is eligible to be paused
        if (!IsActive)
        {
            throw new InvalidOperationException("Can only pause an active subscription.");
        }

        SetStatusPaused(timeProvider);
    }

    public void RequestCancellation(TimeProvider timeProvider)
    {
        // Check if the subscription is already canceled or if a cancellation is already requested
        if (IsCanceled || IsCancellationRequested)
        {
            throw new InvalidOperationException("Cancellation has already been requested or finalized.");
        }

        CancellationRequestedDate = timeProvider.GetUtcNow();
        CancellationStatus = CancellationStatus.CancellationRequested;

        NextRenewalDate = null;
    }

    public void FinalizeCancellation(TimeProvider timeProvider)
    {
        if (CancellationStatus != CancellationStatus.CancellationRequested || !CancellationRequestedDate.HasValue)
        {
            throw new InvalidOperationException("No valid cancellation request to finalize.");
        }

        var currentDate = timeProvider.GetUtcNow();
        var finalizationDate = CancellationRequestedDate.Value.Add(CancellationFinalizationPeriod ?? TimeSpan.Zero);

        // Ensure enough time has passed before finalizing
        if (currentDate >= finalizationDate)
        {
            SetStatusCanceled(timeProvider);
            CancellationStatus = CancellationStatus.Canceled;
            CancellationDate = currentDate;
            EndDate = DateOnly.FromDateTime(currentDate.DateTime);

            NextRenewalDate = null;
        }
        else
        {
            throw new InvalidOperationException("Cancellation cannot be finalized yet. The finalization period has not passed.");
        }
    }

    public void Cancel(string reason, string initiator, TimeProvider timeProvider)
    {
        if (IsCanceled || HasExpired)
        {
            throw new InvalidOperationException("Cannot cancel a subscription that is already canceled or expired.");
        }

        CancellationReason = reason;
        CancellationInitiator = initiator;

        if (CancellationFinalizationPeriod.HasValue && CancellationFinalizationPeriod.Value > TimeSpan.Zero)
        {
            RequestCancellation(timeProvider);
        }
        else
        {
            SetStatusCanceled(timeProvider);
            CancellationStatus = CancellationStatus.Canceled;
            CancellationDate = timeProvider.GetUtcNow();
            EndDate = DateOnly.FromDateTime(timeProvider.GetUtcNow().DateTime);

            NextRenewalDate = null;
        }
    }

    public bool Renew(IBillingDateCalculator billingDateCalculator, IDeliveryDateCalculator deliveryDateCalculator, TimeProvider timeProvider)
    {
        if (IsEligibleForRenewal(TimeSpan.Zero, timeProvider))
        {
            RenewalStatus = RenewalStatus.Renewed;
            SetStatusActive(timeProvider);

            UpdateNextBillingDate(billingDateCalculator);
            UpdateNextDeliveryDate(deliveryDateCalculator);

            LastRenewalDate = timeProvider.GetUtcNow();

            // Calculate the next renewal date based on the renewal frequency and period
            NextRenewalDate = LastRenewalDate.HasValue
                ? CalculateNextRenewalDate(LastRenewalDate.Value, Plan)
                : null;

            return true;
        }

        return false;
    }

    private DateTimeOffset CalculateNextRenewalDate(DateTimeOffset lastRenewalDate, SubscriptionPlan plan)
    {
        return plan.RenewalCycle switch
        {
            RenewalInterval.Days => lastRenewalDate.AddDays(plan.RenewalPeriod),
            RenewalInterval.Weeks => lastRenewalDate.AddDays(7 * plan.RenewalPeriod),
            RenewalInterval.Months => lastRenewalDate.AddMonths(plan.RenewalPeriod),
            RenewalInterval.Years => lastRenewalDate.AddYears(plan.RenewalPeriod),
            _ => throw new InvalidOperationException("Unsupported renewal frequency.")
        };
    }

    private void SetStatusActive(TimeProvider timeProvider)
    {
        if (UpdateStatus(SubscriptionStatusEnum.Active, timeProvider))
        {
            AddDomainEvent(new SubscriptionActivated(TenantId, OrganizationId, Id));
        }
    }

    private void SetStatusCanceled(TimeProvider timeProvider)
    {
        if (UpdateStatus(SubscriptionStatusEnum.Canceled, timeProvider))
        {
            AddDomainEvent(new SubscriptionCanceled(TenantId, OrganizationId, Id));
        }
    }

    private void SetStatusSuspended(TimeProvider timeProvider)
    {
        if (UpdateStatus(SubscriptionStatusEnum.Suspended, timeProvider))
        {
            AddDomainEvent(new SubscriptionSuspended(TenantId, OrganizationId, Id));
        }
    }

    private void SetStatusExpired(TimeProvider timeProvider)
    {
        if (UpdateStatus(SubscriptionStatusEnum.Expired, timeProvider))
        {
            AddDomainEvent(new SubscriptionExpired(TenantId, OrganizationId, Id));

            // Set NextRenewalDate to null as the subscription is no longer renewable
            NextRenewalDate = null;
        }
    }

    private void SetStatusPaused(TimeProvider timeProvider)
    {
        if (UpdateStatus(SubscriptionStatusEnum.Paused, timeProvider))
        {
            AddDomainEvent(new SubscriptionPaused(TenantId, OrganizationId, Id));
        }
    }

    /// <summary>
    /// Checks if the subscription is eligible for renewal within the specified time window and sets the status to RenewalPending if it is.
    /// </summary>
    /// <param name="timeBeforeRenewal">The time window within which the subscription is considered eligible for renewal.</param>
    /// <param name="billingDateCalculator">The billing date calculator used to calculate the next billing date.</param>
    /// <param name="deliveryDateCalculator">The delivery date calculator used to calculate the next delivery date.</param>
    /// <param name="timeProvider">The time provider used to get the current date and time.</param>
    /// <returns>True if the renewal status was set to pending, otherwise false.</returns>
    public bool SetRenewalPendingIfEligible(TimeSpan timeBeforeRenewal, IBillingDateCalculator billingDateCalculator, IDeliveryDateCalculator deliveryDateCalculator, TimeProvider timeProvider)
    {
        if (IsEligibleForRenewal(timeBeforeRenewal, timeProvider))
        {
            RenewalStatus = RenewalStatus.RenewalPending;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Determines whether the subscription is eligible for renewal based on the specified time window.
    /// </summary>
    /// <param name="timeBeforeRenewal">The time window within which the subscription is considered eligible for renewal.</param>
    /// <param name="timeProvider">The time provider used to get the current date and time.</param>
    /// <returns>True if the subscription is eligible for renewal, otherwise false.</returns>
    public bool IsEligibleForRenewal(TimeSpan timeBeforeRenewal, TimeProvider timeProvider)
    {
        // Check if the subscription is active and auto-renew is enabled
        if (IsActive && HasAutoRenewal)
        {
            var currentDate = timeProvider.GetUtcNow();
            var renewalWindowStart = currentDate.Add(timeBeforeRenewal);

            // Ensure NextRenewalDate exists and is within the renewal window
            var eligibleForRenewal = NextRenewalDate.HasValue && NextRenewalDate.Value <= renewalWindowStart;

            return eligibleForRenewal && (RenewalStatus == RenewalStatus.None || RenewalStatus == RenewalStatus.RenewalPending);
        }

        return false;
    }

    public string? GetIneligibilityReasonForRenewal(TimeSpan timeBeforeRenewal, TimeProvider timeProvider)
    {
        if (!IsActive)
        {
            return "Subscription is not active.";
        }

        if (!HasAutoRenewal)
        {
            return "Auto-renew is not enabled.";
        }

        var currentDate = timeProvider.GetUtcNow();

        if (!NextRenewalDate.HasValue)
        {
            return "Next renewal date is not set.";
        }

        if (NextRenewalDate.Value > currentDate.Add(timeBeforeRenewal))
        {
            return $"Renewal is only eligible within {timeBeforeRenewal.Humanize()} before the next renewal date.";
        }

        if (RenewalStatus == RenewalStatus.RenewalPending || RenewalStatus == RenewalStatus.Renewed)
        {
            return "Renewal is already pending or has been renewed.";
        }

        return null;
    }

    public void Suspend(TimeProvider timeProvider)
    {
        // Ensure the subscription is in a state where it can be suspended
        if ((IsActive || IsPaused) && !IsCanceled && !HasExpired)
        {
            SetStatusSuspended(timeProvider);
        }
        else
        {
            throw new InvalidOperationException("Subscription cannot be suspended in its current state.");
        }
    }

    public void ReactivateAfterSuspension(TimeProvider timeProvider)
    {
        if (IsSuspended)
        {
            SetStatusActive(timeProvider);

            // Adjust the renewal date by pushing it forward based on suspension length (?)
            var suspensionPeriod = timeProvider.GetUtcNow() - (LastRenewalDate ?? timeProvider.GetUtcNow());
            NextRenewalDate = NextRenewalDate?.Add(suspensionPeriod);
        }
    }

    public void SetPaymentStatus(PaymentStatus paymentStatus, IBillingDateCalculator billingDateCalculator, IDeliveryDateCalculator deliveryDateCalculator, TimeProvider timeProvider)
    {
        if (IsCanceled || HasExpired)
        {
            throw new InvalidOperationException("Cannot update payment status for a canceled or expired subscription.");
        }

        PaymentStatus = paymentStatus;

        if (HasPaymentSucceeded)
        {
            if (IsTrialActive(timeProvider))
            {
                EndTrial(timeProvider);
            }
            else
            {
                SetStatusActive(timeProvider);
                RenewalStatus = RenewalStatus.Renewed;
                UpdateNextBillingDate(billingDateCalculator);
                UpdateNextDeliveryDate(deliveryDateCalculator);
            }
        }
        else if (HasPaymentFailed)
        {
            if (IsActive || IsTrial || IsPaused)
            {
                SetStatusSuspended(timeProvider);
            }
            else
            {
                throw new InvalidOperationException("Cannot suspend subscription in its current state.");
            }
        }
    }

    public void UpdateNextBillingDate(IBillingDateCalculator billingDateCalculator)
    {
        if (Plan == null)
        {
            throw new InvalidOperationException("Subscription plan information is required to calculate the next billing date.");
        }

        /*
        if (this.BillingFrequency == null) // Assuming BillingFrequency is a property of Plan
        {
            throw new InvalidOperationException("Billing frequency information is required to calculate the next billing date.");
        }
        */

        NextBillingDate = billingDateCalculator.CalculateNextBillingDate(this);
    }

    public void UpdateNextDeliveryDate(IDeliveryDateCalculator deliveryDateCalculator)
    {
        if (Plan == null)
        {
            throw new InvalidOperationException("Subscription plan information is required to calculate the next delivery date.");
        }

        if (Plan.Schedule.Frequency == null) // Assuming Recurrence is a property of Plan or SubscriptionSchedule
        {
            throw new InvalidOperationException("Recurrence information is required to calculate the next delivery date.");
        }

        NextDeliveryDate = deliveryDateCalculator.CalculateNextDeliveryDate(this);

        ClearDeliveryOrderId();
    }


    public DateTimeOffset? TrialStartDate { get; set; }
    public DateTimeOffset? TrialEndDate { get; set; }
    public DateTimeOffset? TrialEndedDate { get; set; }

    public TimeInterval BillingCycle { get; set; } = TimeInterval.Monthly; // New property for billing frequency
    public DateTimeOffset? NextBillingDate { get; set; }
    public BillingStatus BillingStatus { get; set; }

    public DateTimeOffset? NextDeliveryDate { get; set; }
    public int? NextDeliveryOrderId { get; private set; }

    public void SetNextDeliveryOrderId(int orderId)
    {
        if (orderId <= 0)
        {
            throw new ArgumentException("Order ID must be a positive value.", nameof(orderId));
        }

        NextDeliveryOrderId = orderId;
    }

    public void ClearDeliveryOrderId()
    {
        NextDeliveryOrderId = null;
    }

    public SubscriptionSchedule Schedule { get; set; }

    public bool IsPaymentPending => PaymentStatus == PaymentStatus.PaymentPending;
    public bool HasPaymentSucceeded => PaymentStatus == PaymentStatus.PaymentSucceeded;
    public bool HasPaymentFailed => PaymentStatus == PaymentStatus.PaymentFailed;

    public bool IsPending => StatusId == (int)SubscriptionStatusEnum.Pending;
    public bool IsActive => StatusId == (int)SubscriptionStatusEnum.Active;
    public bool IsPaused => StatusId == (int)SubscriptionStatusEnum.Paused;
    public bool IsSuspended => StatusId == (int)SubscriptionStatusEnum.Suspended;
    public bool IsCanceled => StatusId == (int)SubscriptionStatusEnum.Canceled;
    public bool HasExpired => StatusId == (int)SubscriptionStatusEnum.Expired;

    public bool IsTrial => StatusId == (int)SubscriptionStatusEnum.Trial;

    public bool IsCancellationRequested => CancellationStatus == CancellationStatus.CancellationRequested;

    public bool IsRenewalPending => RenewalStatus == RenewalStatus.RenewalPending;
    public bool IsRenewed => RenewalStatus == RenewalStatus.Renewed;

    public bool CanStartTrial => IsPending && !HasUsedTrial;

    public bool CanActivate(TimeProvider timeProvider)
    {
        // Check if the subscription is in trial or pending state and if the payment has succeeded
        return (IsTrialActive(timeProvider) || IsPending) && HasPaymentSucceeded;
    }

    public bool CanPause()
    {
        // Check if the subscription is in an active state
        return IsActive;
    }

    public bool CanRenew => IsActive && RenewalStatus == RenewalStatus.RenewalPending;

    // Check if the subscription is not already canceled, expired, pending, or has a cancellation request
    public bool CanCancel => !IsCanceled && !HasExpired && !IsPending && !IsCancellationRequested;

    public bool CanSuspend => IsActive || IsPaused;

    // Sample method to check if the trial is currently active
    public bool IsTrialActive(TimeProvider timeProvider)
    {
        return IsTrial &&
               TrialEndDate.HasValue &&
               TrialEndDate.Value > timeProvider.GetUtcNow();
    }

    // Property to track if the trial has already been used
    public bool HasUsedTrial { get; set; } = false;

    // Method to determine if the next delivery is part of the trial period
    public bool IsNextDeliveryPartOfTrial(IDeliveryDateCalculator deliveryDateCalculator, TimeProvider timeProvider)
    {
        if (TrialStartDate == null || TrialEndDate == null)
        {
            return false; // No trial period defined
        }

        var nextDeliveryDate = deliveryDateCalculator.CalculateNextDeliveryDate(this);

        // Check if the next delivery date falls within the trial period
        return nextDeliveryDate >= TrialStartDate.Value && nextDeliveryDate <= TrialEndDate.Value;
    }

    public int GetRemainingTrialDeliveries(IDeliveryDateCalculator deliveryDateCalculator, TimeProvider timeProvider)
    {
        if (TrialStartDate == null || TrialEndDate == null)
        {
            return 0; // No trial period defined
        }

        var remainingDeliveries = 0;
        var currentDate = timeProvider.GetUtcNow();
        var nextDeliveryDate = deliveryDateCalculator.CalculateNextDeliveryDate(this, currentDate);

        // Loop through and count deliveries until the trial end date is reached
        while (nextDeliveryDate <= TrialEndDate.Value && nextDeliveryDate >= currentDate)
        {
            remainingDeliveries++;
            nextDeliveryDate = deliveryDateCalculator.CalculateNextDeliveryDate(this, nextDeliveryDate);
        }

        return remainingDeliveries;
    }

    public User? CreatedBy { get; set; }
    public UserId? CreatedById { get; set; }
    public DateTimeOffset Created { get; set; }
    public User? LastModifiedBy { get; set; }
    public UserId? LastModifiedById { get; set; }
    public DateTimeOffset? LastModified { get; set; }

    public bool IsDeleted { get; set; }
    public DateTimeOffset? Deleted { get; set; }
    public UserId? DeletedById { get; set; }
    public User? DeletedBy { get; set; }

}

public sealed record SubscriptionActivated(TenantId TenantId, OrganizationId OrganizationId, Guid SubscriptionId) : DomainEvent;

public sealed record SubscriptionCanceled(TenantId TenantId, OrganizationId OrganizationId, Guid SubscriptionId) : DomainEvent;

public sealed record SubscriptionSuspended(TenantId TenantId, OrganizationId OrganizationId, Guid SubscriptionId) : DomainEvent;

public sealed record SubscriptionPaused(TenantId TenantId, OrganizationId OrganizationId, Guid SubscriptionId) : DomainEvent;

public sealed record SubscriptionExpired(TenantId TenantId, OrganizationId OrganizationId, Guid SubscriptionId) : DomainEvent;


public enum SubscriptionStatusEnum
{
    Pending = 1,
    Active,
    Trial,
    Paused,
    Canceled,
    Expired,
    Suspended
}


public enum CancellationStatus
{
    None,
    CancellationRequested,
    Canceled
}

public enum PaymentStatus
{
    None,
    PaymentPending,
    PaymentFailed,
    PaymentSucceeded
}

public enum RenewalStatus
{
    None,
    RenewalPending,
    RenewalFailed,
    Renewed
}

public interface IBillingDateCalculator
{
    DateTimeOffset CalculateNextBillingDate(Subscription subscription);
    IEnumerable<DateTimeOffset> GenerateUpcomingBillingDates(Subscription subscription);
}

public interface IDeliveryDateCalculator
{
    DateTimeOffset CalculateNextDeliveryDate(Subscription subscription);
    DateTimeOffset CalculateNextDeliveryDate(Subscription subscription, DateTimeOffset fromDate);
    IEnumerable<DateTimeOffset> GenerateUpcomingDeliveryDates(Subscription subscription);
}

public class DefaultBillingDateCalculator(TimeProvider timeProvider) : IBillingDateCalculator
{
    public DateTimeOffset CalculateNextBillingDate(Subscription subscription)
    {
        var baseDate = subscription.NextBillingDate ?? timeProvider.GetUtcNow();
        var schedule = subscription.Schedule;
        return subscription.BillingCycle switch
        {
            TimeInterval.Daily => baseDate.AddDays(schedule.EveryDays ?? 1),
            TimeInterval.Weekly => baseDate.AddDays((schedule.EveryWeeks ?? 1) * 7),
            TimeInterval.Monthly => baseDate.AddMonths(schedule.EveryMonths ?? 1),
            TimeInterval.Quarterly => baseDate.AddMonths(3),
            TimeInterval.Yearly => baseDate.AddYears(schedule.EveryYears ?? 1),
            _ => throw new InvalidOperationException("Unsupported frequency."),
        };
    }

    public IEnumerable<DateTimeOffset> GenerateUpcomingBillingDates(Subscription subscription)
    {
        var dates = new List<DateTimeOffset>();
        var currentDate = subscription.NextBillingDate ?? timeProvider.GetUtcNow();
        var endDateTimeOffset = subscription.EndDate.HasValue
            ? subscription.EndDate.Value.ToDateTime(new TimeOnly(0, 0)).ToUniversalTime()
            : (DateTimeOffset?)null;

        while (!endDateTimeOffset.HasValue || currentDate < endDateTimeOffset.Value)
        {
            dates.Add(currentDate);
            currentDate = CalculateNextBillingDate(subscription);
        }

        return dates;
    }
}

public class DefaultDeliveryDateCalculator(TimeProvider timeProvider) : IDeliveryDateCalculator
{
    public DateTimeOffset CalculateNextDeliveryDate(Subscription subscription)
    {
        var baseDate = subscription.NextDeliveryDate ?? timeProvider.GetUtcNow();
        return CalculateNextDeliveryDate(subscription, baseDate);
    }

    public DateTimeOffset CalculateNextDeliveryDate(Subscription subscription, DateTimeOffset fromDate)
    {
        return subscription.Schedule.GetNextDate(fromDate);
    }

    public IEnumerable<DateTimeOffset> GenerateUpcomingDeliveryDates(Subscription subscription)
    {
        var dates = new List<DateTimeOffset>();
        var currentDate = subscription.NextBillingDate ?? timeProvider.GetUtcNow();
        var endDateTimeOffset = subscription.EndDate.HasValue
            ? subscription.EndDate.Value.ToDateTime(new TimeOnly(0, 0)).ToUniversalTime()
            : (DateTimeOffset?)null;

        while (!endDateTimeOffset.HasValue || currentDate < endDateTimeOffset.Value)
        {
            dates.Add(currentDate);
            currentDate = CalculateNextDeliveryDate(subscription, currentDate);
        }

        return dates;
    }
}