using YourBrand.Orders.Domain.Common;

namespace YourBrand.Orders.Domain.Entities
{
    public class OrderStatus : AuditableEntity, ISoftDelete
    {
        public OrderStatus()
        {

        }

        public OrderStatus(string id, string name, string? description = null)
        {
            Id = id;
            Name = name;
            Description = description;
        }

        public string Id { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public DateTime? Deleted { get; set; }

        public string? DeletedById { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}