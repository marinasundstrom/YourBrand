using YourBrand.Domain;
using YourBrand.Transactions.Domain.Common;

namespace YourBrand.Transactions.Domain.Events;

public record TransactionReferenceUpdated : DomainEvent
{
    public TransactionReferenceUpdated(OrganizationId organizationId, string transactionId, string reference)
    {
        OrganizationId = organizationId;
        TransactionId = transactionId;
        Reference = reference;
    }

    public OrganizationId OrganizationId { get; }
    
    public string TransactionId { get; }

    public string Reference { get; }
}