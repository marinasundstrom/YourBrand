using System.ComponentModel.DataAnnotations;

namespace Catalog.WebApi.Controllers;

public class PostCommentDto
{
    [Required]
    public string Text { get; set; } = null!;
}