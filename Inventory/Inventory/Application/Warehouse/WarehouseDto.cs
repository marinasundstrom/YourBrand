using System.ComponentModel.DataAnnotations;

using YourBrand.Inventory.Application;
using YourBrand.Inventory.Application.Common.Models;
using YourBrand.Inventory.Application.Sites;

namespace YourBrand.Inventory.Application.Warehouses;

public record WarehouseDto
(
    string Id,
    string Name,
    SiteDto Site
);
