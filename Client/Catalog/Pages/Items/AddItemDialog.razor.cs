using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

using MudBlazor;

using System.ComponentModel.DataAnnotations;

namespace Catalog.Pages.Items
{
    public partial class AddItemDialog
    {
        FormModel model = new FormModel();
        byte[]? imageBytes = null;
        bool success;
        
        public class FormModel
        {
            [Required]
            [StringLength(60, ErrorMessage = "Name length can't be more than 8.")]
            public string Name { get; set; } = null !;
            [Required]
            [StringLength(240, ErrorMessage = "Description length can't be more than 240.")]
            public string Description { get; set; } = null !;
            public Stream? Stream { get; set; }
        }

        [CascadingParameter]
        MudDialogInstance MudDialog { get; set; }

        void OnValidSubmit() => MudDialog.Close(DialogResult.Ok(model));
        void Cancel() => MudDialog.Cancel();
        private async Task UploadFiles(InputFileChangeEventArgs e)
        {
            if (e.File.Size > Constants.FileMaxSize)
            {
                Snackbar.Add("Image is too big.", Severity.Error);
                return;
            }

            var stream = e.File.OpenReadStream(Constants.FileMaxSize);
            await Process(stream);
        }

        private async Task Process(Stream stream)
        {
            var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);
            imageBytes = await GetBytes(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);
            Console.WriteLine(memoryStream.Length);
            model.Stream = memoryStream;
        }

        private async Task<byte[]> GetBytes(MemoryStream memoryStream)
        {
            var imageBytes = memoryStream.ToArray();
            return imageBytes;
        }
    }
}