namespace BlazorApp.Products;

public class OptionGroupVM
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int? Min { get; set; }

    public int? Max { get; set; }

    public List<OptionVM> Options { get; set; } = new List<OptionVM>();
}