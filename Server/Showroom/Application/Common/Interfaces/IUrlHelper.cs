namespace YourBrand.Showroom.Application.Common.Interfaces;

public interface IUrlHelper
{
    string GetHostUrl();

    string? CreateImageUrl(string? id);
}