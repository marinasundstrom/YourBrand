using System.ComponentModel.DataAnnotations;

namespace YourBrand.Meetings.Motions.MotionDetails;

public class OperativeClauseViewModel
{
    public string Id { get; set; }

    public int Order { get; set; }

    [Required]
    public OperativeAction Action { get; set; }

    [Required]
    public string Text { get; set; }

    public OperativeClauseViewModel Clone()
    {
        return new OperativeClauseViewModel
        {
            Id = Id,
            Order = Order,
            Action = Action,
            Text = Text
        };
    }
}