using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp.Data;

public sealed class ApplicationDbContext(DbContextOptions options) : IdentityDbContext<IdentityUser>(options)
{
    public DbSet<Movie> Movies { get; set; } = default!;
}