using YourBrand.Domain;

namespace YourBrand;

public static class OrganizationExtensions
{
    public static IQueryable<T> InOrganization<T>(this IQueryable<T> query, OrganizationId organizationId)
        where T : IHasOrganization
    {
        return query.Where(x => x.OrganizationId == organizationId);
    }

    public static IEnumerable<T> InOrganization<T>(this IEnumerable<T> query, OrganizationId organizationId)
      where T : IHasOrganization
    {
        return query.Where(x => x.OrganizationId == organizationId);
    }
}