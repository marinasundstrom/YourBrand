using System.Text.Json.Serialization;

namespace YourBrand.Showroom.Domain.Entities;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ExperienceType 
{
    Employment,
    Assignment,
    Project,
    Role,
    CareerBreak
}