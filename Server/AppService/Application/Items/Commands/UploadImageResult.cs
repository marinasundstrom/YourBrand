using System.Text.Json.Serialization;

using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json.Converters;

namespace YourBrand.Application.Items.Commands;

public enum UploadImageResult
{
    Successful,
    NotFound
}