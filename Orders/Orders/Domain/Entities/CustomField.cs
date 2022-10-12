using Microsoft.EntityFrameworkCore;

using YourBrand.Orders.Domain.Common;

namespace YourBrand.Orders.Domain.Entities
{
    public class CustomField : AuditableEntity
    {
        public Guid Id { get; set; }

        public Order? Order { get; set; }

        public OrderItem? OrderItem { get; set; }

        public CustomFieldDefinition Definition { get; set; } = null!;

        public string CustomFieldId { get; set; } = null!;

        public string Value { get; set; } = null!;
    }
}