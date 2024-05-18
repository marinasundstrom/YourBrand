using HealthChecks.SqlServer;

public static class SqlResourceExtensions
{
    /// <summary>
    /// Adds a health check to the SQL Server server resource.
    /// </summary>
    public static IResourceBuilder<SqlServerServerResource> WithHealthCheck(this IResourceBuilder<SqlServerServerResource> builder)
    {
        return builder.WithAnnotation(new HealthCheckAnnotation(cs => new SqlServerHealthCheck(new SqlServerHealthCheckOptions { ConnectionString = cs })));
    }

    /// <summary>
    /// Adds a health check to the SQL Server database resource.
    /// </summary>
    public static IResourceBuilder<SqlServerDatabaseResource> WithHealthCheck(this IResourceBuilder<SqlServerDatabaseResource> builder)
    {
        return builder.WithAnnotation(new HealthCheckAnnotation(cs => new SqlServerHealthCheck(new SqlServerHealthCheckOptions { ConnectionString = cs })));
    }
}