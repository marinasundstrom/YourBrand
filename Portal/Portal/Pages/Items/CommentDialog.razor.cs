using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

using MudBlazor;

using YourBrand.AppService.Client;

using System.ComponentModel.DataAnnotations;

namespace YourBrand.Portal.Pages.Items;

public partial class CommentDialog
{
    FormModel model = new FormModel();
    bool success;
    
    public class FormModel
    {
        [Required]
        [StringLength(1024, ErrorMessage = "Text length can't be more than 1024.")]
        public string Text { get; set; } = null !;
    }

    [CascadingParameter] MudDialogInstance MudDialog { get; set; }

    [Parameter] public string ItemId { get; set; }

    [Parameter] public string? CommentId { get; set; }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            if (CommentId is not null)
            {
                var comment = await ItemsClient.GetCommentAsync(ItemId, CommentId);
                model.Text = comment.Text;
            }
        }
        catch (AccessTokenNotAvailableException exception)
        {
            exception.Redirect();
        }
    }

    async Task OnValidSubmit()
    {
        try
        {
            if (CommentId is null)
            {
                await ItemsClient.PostCommentAsync(ItemId, new PostCommentDto()
                {
                    Text = model.Text
                });
            }
            else
            {
                await ItemsClient.UpdateCommentAsync(ItemId, CommentId, new UpdateCommentDto()
                {
                    Text = model.Text
                });
            }
        }
        catch (AccessTokenNotAvailableException exception)
        {
            exception.Redirect();
        }
        catch (Exception exc)
        {
            Snackbar.Add(exc.Message.ToString(), Severity.Error);
        }

        MudDialog.Close(DialogResult.Ok(model));
    }

    void Cancel() => MudDialog.Cancel();
}