using System.ComponentModel.DataAnnotations;

namespace YourBrand.WebApi.Controllers;

public class UpdateCommentDto
{
    [Required]
    public string Text { get; set; } = null!;
}
