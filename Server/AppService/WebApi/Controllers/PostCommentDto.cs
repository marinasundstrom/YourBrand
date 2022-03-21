using System.ComponentModel.DataAnnotations;

namespace YourCompany.WebApi.Controllers;

public class PostCommentDto
{
    [Required]
    public string Text { get; set; } = null!;
}
