﻿using YourBrand.Domain;
using YourBrand.Payments.Domain.Common;
using YourBrand.Payments.Domain.Enums;
using YourBrand.Payments.Domain.Events;
using YourBrand.Tenancy;

namespace YourBrand.Payments.Domain.Entities;

public class Payment : AuditableEntity<string>, IHasTenant, IHasOrganization
{
    readonly HashSet<Capture> _captures = new HashSet<Capture>();

    private Payment()
    {

    }

    public Payment(string organizationId, string invoiceId, PaymentStatus status, string currency, decimal amount, DateTime dueDate, PaymentMethod paymentMethod, string? reference = null, string? message = null)
        : base(Guid.NewGuid().ToString())
    {
        if (amount <= 0)
        {
            throw new ArgumentException("Amount must be greater than 0.");
        }

        OrganizationId = organizationId;
        InvoiceId = invoiceId;
        Status = status;
        DueDate = dueDate;
        Currency = currency;
        Amount = amount;
        PaymentMethod = paymentMethod;
        Reference = reference;
        Message = message;

        AddDomainEvent(new PaymentCreated(Id));
    }

    public void SetStatus(PaymentStatus status)
    {
        if (Status != status)
        {
            Status = status;

            AddDomainEvent(new PaymentStatusChanged(Id, status));
        }
    }

    public TenantId TenantId { get; set; }

    public OrganizationId OrganizationId { get; set; }

    public string InvoiceId { get; private set; }

    public PaymentStatus Status { get; private set; } = PaymentStatus.Created;

    public DateTime DueDate { get; private set; }

    public string Currency { get; private set; } = null!;

    public decimal Amount { get; private set; }

    public PaymentMethod PaymentMethod { get; private set; }

    public string? Reference { get; private set; }

    public string? Message { get; private set; }

    public decimal? AmountCaptured { get; private set; }

    public IReadOnlyCollection<Capture> Captures => _captures;

    public void RegisterCapture(DateTime date, decimal amount, string? transactionId)
    {
        var capture = new Capture(date, amount, transactionId);

        _captures.Add(capture);

        AmountCaptured = Captures.Sum(c => c.Amount);

        capture.AddDomainEvent(new PaymentCaptured(Id, capture.Id));
    }
}