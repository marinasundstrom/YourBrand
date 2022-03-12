using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using Skynet.Showroom.Application.Common.Interfaces;
using Skynet.Showroom.Application.ConsultantProfiles;
using Skynet.Showroom.Application.CompetenceAreas;
using Skynet.Showroom.Application.Users;
using Skynet.Showroom.Application.Common.Models;
using Skynet.Showroom.Application.Organizations;
using Skynet.Showroom.Application.ConsultantProfiles.Experiences;

namespace Skynet.Showroom.Application;

public static class Mapper
{
    public static UserDto ToDto(this Domain.Entities.User user)
    {
        return new UserDto(user.Id, user.FirstName, user.LastName, user.DisplayName, user.SSN, user.Email, user.Created, user.Deleted);
    }

    public static ConsultantProfileDto ToDto(this Domain.Entities.ConsultantProfile consultantProfile, IUrlHelper urlHelper)
    {
        return new ConsultantProfileDto(
            consultantProfile.Id,
            consultantProfile.FirstName,
            consultantProfile.LastName,
            consultantProfile.DisplayName,
            consultantProfile.Organization.ToDto(),
            consultantProfile.CompetenceArea.ToDto(),
            consultantProfile.ProfileImage,
            consultantProfile.Headline,
            consultantProfile.ShortPresentation,
            consultantProfile.Presentation,
            consultantProfile.ProfileVideo,
            consultantProfile.AvailableFromDate,
            consultantProfile.Email,
            consultantProfile.PhoneNumber);
    }

    public static CompetenceAreaDto ToDto(this Domain.Entities.CompetenceArea competenceArea)
    {
        return new CompetenceAreaDto(competenceArea.Id, competenceArea.Name);
    }

    public static OrganizationDto ToDto(this Domain.Entities.Organization organization)
    {
        // organization.Id, organization.Name, competenceArea.Created, competenceArea.CreatedBy?.ToDto(), competenceArea.LastModified, competenceArea.LastModifiedBy?.ToDto(

        return new OrganizationDto(organization.Id, organization.Name, organization.Address.ToDto());
    }

    public static AddressDto ToDto(this Domain.Entities.Address address)
    {
        return new AddressDto(address.Address1, address.Address2, address.PostalCode, address.Locality, address.SubAdminArea, address.AdminArea, address.Country);
    }

    public static ExperienceDto ToDto(this Domain.Entities.ConsultantProfileExperience experience)
    {
        return new ExperienceDto(experience.Id, experience.Title, experience.CompanyName, experience.Location, experience.StartDate, experience.EndDate, experience.Description);
    }
}