using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

using MudBlazor;

namespace YourBrand.ChatApp.Chat.Channels;

public partial class NewChannelDialog
{
    readonly FormModel model = new FormModel();

    public class FormModel
    {
        [Required]
        [StringLength(60, ErrorMessage = "Title length can't be more than 8.")]
        public string Name { get; set; } = null!;
    }

    [CascadingParameter]
    MudDialogInstance MudDialog { get; set; } = default!;

    [CascadingParameter(Name = "Organization")]
    public YourBrand.Portal.Services.Organization Organization { get; set; }

    async Task OnValidSubmit()
    {
        var channel = await ChannelsClient.CreateChannelAsync(Organization.Id, new CreateChannelRequest() { Name = model.Name });
        MudDialog.Close(DialogResult.Ok(channel));
    }
}