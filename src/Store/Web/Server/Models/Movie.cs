using System.ComponentModel.DataAnnotations;

namespace BlazorApp;

public class Movie
{
    public int Id { get; set; }

    [Required]
    public string? Title { get; set; }

    public DateOnly? ReleaseDate { get; set; }

    [Required]
    public string? Genre { get; set; }

    [Required]
    public decimal Price { get; set; }
}