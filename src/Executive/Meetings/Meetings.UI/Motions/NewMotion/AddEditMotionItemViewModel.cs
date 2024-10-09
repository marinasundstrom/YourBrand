using System.ComponentModel.DataAnnotations;

namespace YourBrand.Meetings.Motions.NewMotion;

public class AddEditMotionItemViewModel
{
    [Required]
    public string Text { get; set; }

    public AddEditMotionItemViewModel Clone() 
    {
        return new AddEditMotionItemViewModel 
        {
            Text = Text
        };
    }
}