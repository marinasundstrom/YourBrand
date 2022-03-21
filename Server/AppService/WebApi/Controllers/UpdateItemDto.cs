using System.ComponentModel.DataAnnotations;

namespace YourCompany.WebApi.Controllers;

public class UpdateItemDto
{
    [Required]
    public string Name { get; set; } = null!;

    [Required]
    public string Description { get; set; } = null!;
}