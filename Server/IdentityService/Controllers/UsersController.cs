using IdentityService.Data;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityService;

[Route("[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class UsersController : Controller
{
    private readonly ApplicationDbContext _context;

    public UsersController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
    {
        return await _context.Users
            .Include(u => u.Department)
            .Select(u => new UserDto {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                Department = u.Department == null ? null : new DepartmentDto {
                    Id = u.Department.Id,
                    Name = u.Department.Name
                }
            }).ToArrayAsync();
    }
}

public class UserDto 
{
    public string Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public DepartmentDto? Department { get; set; }
}

public class DepartmentDto 
{
    public string Id { get; set; }

    public string Name { get; set; }
}

public class TeamDto 
{
    public string Id { get; set; }

    public string Name { get; set; }
}