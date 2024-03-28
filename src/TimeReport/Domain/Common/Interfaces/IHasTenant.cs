
namespace YourBrand.TimeReport.Domain.Common.Interfaces;

public interface IHasTenant
{
    public string OrganizationId { get; set; }
}