using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Domain.Entities;

namespace YourBrand.TimeReport.Application;

public static class IncludeExtensions 
{
    public static IQueryable<Activity> IncludeAll(this IQueryable<Activity> source)
    {
        return source
            .Include(a => a.CreatedBy)
            .Include(a => a.LastModifiedBy)
            .Include(a => a.DeletedBy);
    }

    public static IQueryable<Organization> IncludeAll(this IQueryable<Organization> source)
    {
        return source
            .Include(o => o.CreatedBy)
            .Include(o => o.LastModifiedBy)
            .Include(o => o.DeletedBy);
    }

    public static IQueryable<ExpenseType> IncludeAll(this IQueryable<ExpenseType> source)
    {
        return source
            .Include(et => et.Project)
            .Include(et => et.CreatedBy)
            .Include(et => et.LastModifiedBy)
            .Include(et => et.DeletedBy);
    }

    public static IQueryable<AbsenceType> IncludeAll(this IQueryable<AbsenceType> source)
    {
        return source
            .Include(at => at.CreatedBy)
            .Include(at => at.LastModifiedBy)
            .Include(at => at.DeletedBy);
    }

    public static IQueryable<Project> IncludeAll(this IQueryable<Project> source)
    {
        return source
            .Include(p => p.CreatedBy)
            .Include(p => p.LastModifiedBy)
            .Include(p => p.DeletedBy);
    }

    public static IQueryable<ProjectMembership> IncludeAll(this IQueryable<ProjectMembership> source)
    {
        return source
            .Include(pm => pm.CreatedBy)
            .Include(pm => pm.LastModifiedBy)
            .Include(pm => pm.DeletedBy);
    }

    public static IQueryable<ProjectGroup> IncludeAll(this IQueryable<ProjectGroup> source)
    {
        return source
            .Include(pg => pg.CreatedBy)
            .Include(pg => pg.LastModifiedBy)
            .Include(pg => pg.DeletedBy);
    }

    public static IQueryable<ActivityType> IncludeAll(this IQueryable<ActivityType> source)
    {
        return source
            .Include(at => at.Project)
            .Include(at => at.CreatedBy)
            .Include(at => at.LastModifiedBy)
            .Include(at => at.DeletedBy);
    }
}