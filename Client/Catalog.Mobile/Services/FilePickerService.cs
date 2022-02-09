using System;

using MudBlazor;

namespace Catalog.Services;

public class FilePickerService : IFilePickerService
{
    public FilePickerService(ISnackbar snackbar)
    {
        Snackbar = snackbar;
    }

    public ISnackbar Snackbar { get; }

    public async Task<Stream?> PickImage()
    {
        var r = await App.Current.MainPage.DisplayActionSheet("Choose an image", "Cancel", "Cancel", new[]
        {
            "Photos",
            "Files"
        });

        FileResult fileResult = null;

        switch (r)
        {
            case "Photos":
                fileResult = await MediaPicker.PickPhotoAsync(new MediaPickerOptions()
                {
                    Title = "Pick an image"
                });
                break;

            case "Files":

                var customFileType = FilePickerFileType.Images;

                var options = new PickOptions
                {
                    PickerTitle = "Please select an image",
                    FileTypes = customFileType,
                };

                fileResult = await FilePicker.PickAsync(options);
                break;
        }

        if (fileResult is null) return null;

        var stream = await fileResult.OpenReadAsync();

        if (stream.Length > Constants.FileMaxSize)
        {
            Snackbar.Add("Image is too big.", Severity.Error);

            return null;
        }

#if IOS
        stream = await iOS.ImageConverter.ConvertToJpegAsync(stream);
#elif MACCATALYST
        stream = await MacCatalyst.ImageConverter.ConvertToJpegAsync(stream);
#endif

        return stream;
    }
}