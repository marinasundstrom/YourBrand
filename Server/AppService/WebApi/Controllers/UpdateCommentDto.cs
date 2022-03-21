using System.ComponentModel.DataAnnotations;

namespace YourCompany.WebApi.Controllers;

public class UpdateCommentDto
{
    [Required]
    public string Text { get; set; } = null!;
}
