using System.ComponentModel.DataAnnotations;

using YourBrand.Showroom.Client;

namespace YourBrand.Showroom.Persons;


public class ExperienceSelectorVm
{
    [Required]
    public IndustryDto? Industry { get; set; }

    //[Required]
    public int? Years { get; set; } = 1;

    public List<IndustryVM> Industries { get; set; } = new List<IndustryVM>();

    public void AddIndustry()
    {
        Industries.Add(new IndustryVM()
        {
            Industry = Industry!,
            Years = Years,
            Selected = true
        });

        Industry = null;
        Years = 1;
    }

    public void RemoveIndustry(IndustryVM industry)
    {
        Industries.Remove(industry);
    }
}

public class IndustryVM
{
    public IndustryDto Industry { get; set; }

    public int? Years { get; set; } = 1;

    public bool Selected { get; set; }
}