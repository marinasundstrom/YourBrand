using System.ComponentModel.DataAnnotations;

namespace YourBrand.WebApi.Controllers;

public class PostCommentDto
{
    [Required]
    public string Text { get; set; } = null!;
}
