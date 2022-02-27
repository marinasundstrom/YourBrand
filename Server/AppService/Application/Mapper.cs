using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using Skynet.Application.Users;

namespace Skynet.Application;

public static class Mapper
{
    public static UserDto ToDto(this Domain.Entities.User user)
    {
        return new UserDto(user.Id, user.FirstName, user.LastName, user.DisplayName, user.SSN, user.Email, user.Created, user.Deleted);
    }
}