using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using YourBrand.IdentityService.Application.Users;
using YourBrand.IdentityService.Domain.Entities;

namespace YourBrand.IdentityService.Application;

public static class Mapper
{
    public static UserDto ToDto(this Person user ) => new UserDto(user.Id, user.FirstName, user.LastName, user.DisplayName, user.Roles.First().Name, user.SSN, user.Email,
                    user.Created, user.LastModified);

}