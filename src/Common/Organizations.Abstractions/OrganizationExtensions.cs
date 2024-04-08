namespace YourBrand.Domain;

public static class OrganizationExtensions
{
    public static IQueryable<T> WithOrganization<T>(this IQueryable<T> query, OrganizationId organizationId)
        where T : IHasOrganization
    {
        return query.Where(x => x.OrganizationId == organizationId);
    }

    public static IEnumerable<T> WithOrganization<T>(this IEnumerable<T> query, OrganizationId organizationId)
      where T : IHasOrganization
    {
        return query.Where(x => x.OrganizationId == organizationId);
    }
}