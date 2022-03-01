using System.ComponentModel.DataAnnotations;

namespace Skynet.WebApi.Controllers;

public class UpdateCommentDto
{
    [Required]
    public string Text { get; set; } = null!;
}
