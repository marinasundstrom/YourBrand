using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Components;

using MudBlazor;

namespace YourBrand.Messenger.Messages;

public partial class NewConversationDialog
{
    readonly FormModel model = new FormModel();

    public class FormModel
    {
        [Required]
        [StringLength(60, ErrorMessage = "Title length can't be more than 8.")]
        public string Title { get; set; } = null!;
    }

    [CascadingParameter]
    MudDialogInstance MudDialog { get; set; }

    void OnValidSubmit() => MudDialog.Close(DialogResult.Ok(model));
}