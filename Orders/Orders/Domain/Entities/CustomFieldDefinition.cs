using YourBrand.Orders.Domain.Common;

namespace YourBrand.Orders.Domain.Entities
{
    public class CustomFieldDefinition : AuditableEntity
    {
        public string Id { get; set; } = null!;

        public string Name { get; set; } = null!;

        public CustomFieldType Type { get; set; }
    }
}