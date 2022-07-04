using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;

using YourBrand.Orders.Domain.Common;

namespace YourBrand.Orders.Domain.ValueObjects
{
    [Owned]
    public class BillingDetails : ValueObject
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? Ssn { get; set; }

        public string? OrganizationName { get; set; }
        public string? organizationNo { get; set; }
        public string? VatNo { get; set; }

        public Address Address { get; set; } = null!;
        //public PaymentDetails Payment { get; set; } = null!;

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return FirstName;
            yield return LastName;
            yield return Ssn!;
            yield return OrganizationName!;
            yield return organizationNo!;
            yield return VatNo!;
            yield return Address;
        }
    }
}