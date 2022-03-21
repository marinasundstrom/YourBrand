using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using YourCompany.Application.Common.Interfaces;
using YourCompany.Application.Items;
using YourCompany.Application.Users;

namespace YourCompany.Application;

public static class Mapper
{
    public static UserDto ToDto(this Domain.Entities.User user)
    {
        return new UserDto(user.Id, user.FirstName, user.LastName, user.DisplayName, user.SSN, user.Email, user.Created, user.Deleted);
    }

    public static ItemDto ToDto(this Domain.Entities.Item item, IUrlHelper urlHelper)
    {
        return new ItemDto(item.Id, item.Name, item.Description, urlHelper.CreateImageUrl(item.Image), item.CommentCount, item.Created, item.CreatedBy!.ToDto()!, item.LastModified, item.LastModifiedBy?.ToDto());
    }

    public static CommentDto ToDto(this Domain.Entities.Comment comment)
    {
        return new CommentDto(comment.Id, comment.Text, comment.Created, comment.CreatedBy?.ToDto(), comment.LastModified, comment.LastModifiedBy?.ToDto());
    }
}