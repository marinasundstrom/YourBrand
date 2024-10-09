using System.ComponentModel.DataAnnotations;

namespace YourBrand.Meetings.Motions.MotionDetails;

public class MotionItemViewModel
{
    public string Id { get; set; }

    [Required]
    public string Text { get; set; }

    public MotionItemViewModel Clone()
    {
        return new MotionItemViewModel
        {
            Text = Text
        };
    }
}