using YourBrand.Orders.Domain.Entities;

namespace YourBrand.Orders.Infrastructure.Persistence;

public class SeedData
{
    public static async Task EnsureSeedData(WebApplication app)
    {
        using (var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<SeedData>>();

            var context = scope.ServiceProvider.GetRequiredService<OrdersContext>();
            await context.Database.EnsureDeletedAsync();
            //context.Database.Migrate();
            await context.Database.EnsureCreatedAsync();

            if (!context.OrderStatuses.Any())
            {
                context.OrderStatuses.Add(new OrderStatus("draft", "Draft"));
                context.OrderStatuses.Add(new OrderStatus("placed", "Placed"));
                context.OrderStatuses.Add(new OrderStatus("cancelled", "Cancelled"));
                context.OrderStatuses.Add(new OrderStatus("paid", "Paid"));
            }

            if (!context.CustomFieldDefinitions.Any())
            {
                context.CustomFieldDefinitions.Add(new CustomFieldDefinition
                {
                    Id = "serialNo",
                    Name = "Serial number",
                    Type = CustomFieldType.String
                });

                context.CustomFieldDefinitions.Add(new CustomFieldDefinition
                {
                    Id = "regularPrice",
                    Name = "Regular price",
                    Type = CustomFieldType.String
                });

                context.CustomFieldDefinitions.Add(new CustomFieldDefinition
                {
                    Id = "objectId",
                    Name = "ObjectId",
                    Type = CustomFieldType.String
                });

                context.CustomFieldDefinitions.Add(new CustomFieldDefinition
                {
                    Id = "orderNo",
                    Name = "OrderNo",
                    Type = CustomFieldType.Integer
                });

                context.CustomFieldDefinitions.Add(new CustomFieldDefinition
                {
                    Id = "customerNo",
                    Name = "CustomerNo",
                    Type = CustomFieldType.Integer
                });
            }

            await context.SaveChangesAsync();
        }
    }
}