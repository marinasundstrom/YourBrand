using System.ComponentModel.DataAnnotations;

using YourBrand.Inventory.Application;
using YourBrand.Inventory.Application.Common.Models;

namespace YourBrand.Inventory.Application.Sites;

public record SiteDto
(
    string Id,
    string Name
);
