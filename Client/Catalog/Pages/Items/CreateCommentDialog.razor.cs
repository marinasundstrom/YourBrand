using Microsoft.AspNetCore.Components;

using MudBlazor;

using System.ComponentModel.DataAnnotations;

namespace Catalog.Pages.Items
{
    public partial class CreateCommentDialog
    {
        FormModel model = new FormModel();
        bool success;
        
        public class FormModel
        {
            [Required]
            [StringLength(1024, ErrorMessage = "Text length can't be more than 1024.")]
            public string Text { get; set; } = null !;
        }

        [CascadingParameter]
        MudDialogInstance MudDialog { get; set; }

        void OnValidSubmit() => MudDialog.Close(DialogResult.Ok(model));
        void Cancel() => MudDialog.Cancel();
    }
}