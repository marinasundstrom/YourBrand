using System.ComponentModel.DataAnnotations;

namespace Skynet.WebApi.Controllers;

public class PostCommentDto
{
    [Required]
    public string Text { get; set; } = null!;
}