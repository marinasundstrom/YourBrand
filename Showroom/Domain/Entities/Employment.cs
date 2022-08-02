namespace YourBrand.Showroom.Domain.Entities;

public class Employment
{
    public string Id { get; set; } = null!;

    public ConsultantProfile ConsultantProfile { get; set; } = null!;

    public Company Employer { get; set; } = null!;

    public string Title { get; set; } = null!;

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }
}