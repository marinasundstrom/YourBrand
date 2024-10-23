using System.ComponentModel.DataAnnotations;

namespace YourBrand.Meetings.Motions.NewMotion;

public class AddEditOperativeClauseViewModel
{
    public int Order { get; set; }

    [Required]
    public OperativeAction Action { get; set; }

    [Required]
    public string Text { get; set; }

    public AddEditOperativeClauseViewModel Clone()
    {
        return new AddEditOperativeClauseViewModel
        {
            Order = Order,
            Action = Action,
            Text = Text
        };
    }
}