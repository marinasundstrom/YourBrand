namespace YourBrand.Application.Common.Interfaces;

public interface IUrlHelper
{
    string GetHostUrl();

    string? CreateImageUrl(string? id);
}