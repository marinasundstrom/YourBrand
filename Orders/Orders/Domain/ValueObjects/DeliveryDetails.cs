using System.Collections.Generic;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

using YourBrand.Orders.Domain.Common;

namespace YourBrand.Orders.Domain.ValueObjects
{
    [Owned]
    public class DeliveryDetails : ValueObject, ICloneable
    {
        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public Address Address { get; set; } = null!;

        object ICloneable.Clone()
        {
            return Clone();
        }

        public DeliveryDetails Clone()
        {
            return JsonSerializer.Deserialize<DeliveryDetails>(
                JsonSerializer.Serialize(this))!;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return FirstName;
            yield return LastName;
            yield return Address;
        }
    }
}