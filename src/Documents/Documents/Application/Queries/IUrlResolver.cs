using YourBrand.Documents.Domain.Entities;

namespace YourBrand.Documents.Application.Queries;

public interface IUrlResolver
{
    string GetUrl(Document document);
}
