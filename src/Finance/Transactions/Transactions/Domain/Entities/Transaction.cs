﻿using YourBrand.Domain;
using YourBrand.Tenancy;
using YourBrand.Transactions.Domain.Common;
using YourBrand.Transactions.Domain.Enums;
using YourBrand.Transactions.Domain.Events;

namespace YourBrand.Transactions.Domain.Entities;

public class Transaction : Entity, IHasTenant, IHasOrganization
{
    private Transaction()
    {

    }

    public Transaction(string? id, OrganizationId organizationId, DateTime date, TransactionStatus status, string? from, string? reference, string currency, decimal amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentException("Amount must be greater than 0.");
        }

        Id = id ?? Guid.NewGuid().ToUrlFriendlyString();
        OrganizationId = organizationId;
        Date = date;
        Status = status;
        From = from;
        Reference = reference;
        Currency = currency;
        Amount = amount;

        AddDomainEvent(new TransactionRegistered(Id));
    }

    public string Id { get; set; } = null!;

    public TenantId TenantId { get; set; }

    public OrganizationId OrganizationId { get; set; }

    public DateTime Date { get; set; }

    public TransactionStatus Status { get; private set; } = TransactionStatus.Unverified;

    public void UpdateReference(string reference)
    {
        if (Reference != reference)
        {
            Reference = reference;
            AddDomainEvent(new TransactionReferenceUpdated(OrganizationId, Id, reference));
        }
    }

    public string? From { get; set; }

    public string? Reference { get; set; }

    public string Currency { get; set; } = null!;

    public decimal Amount { get; set; }

    public void SetStatus(TransactionStatus status)
    {
        if (Status != status)
        {
            Status = status;
            AddDomainEvent(new TransactionStatusChanged(Id, status));
        }
    }
}