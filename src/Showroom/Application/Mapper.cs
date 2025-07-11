using YourBrand.Showroom.Application.Cases;
using YourBrand.Showroom.Application.Common.Interfaces;
using YourBrand.Showroom.Application.Common.Models;
using YourBrand.Showroom.Application.Companies;
using YourBrand.Showroom.Application.CompetenceAreas;
using YourBrand.Showroom.Application.Industries;
using YourBrand.Showroom.Application.Organizations;
using YourBrand.Showroom.Application.PersonProfiles;
using YourBrand.Showroom.Application.PersonProfiles.Employments;
using YourBrand.Showroom.Application.PersonProfiles.Experiences;
using YourBrand.Showroom.Application.PersonProfiles.Skills.Queries;
using YourBrand.Showroom.Application.Skills;
using YourBrand.Showroom.Application.Users;
using YourBrand.Showroom.Domain.Entities;

namespace YourBrand.Showroom.Application;

public static class Mapper
{
    public static UserDto ToDto(this Domain.Entities.User user)
    {
        return new (user.Id, user.FirstName, user.LastName, user.DisplayName, user.SSN, user.Email, user.Created, user.Deleted);
    }

    public static PersonProfileDto ToDto(this Domain.Entities.PersonProfile personProfile, IUrlHelper urlHelper)
    {
        return new (
            personProfile.Id,
            personProfile.FirstName,
            personProfile.LastName,
            personProfile.DisplayName,
            personProfile.BirthDate,
            personProfile.Location,
            personProfile.Industry.ToDto(),
            personProfile.Organization.ToDto(),
            personProfile.CompetenceArea.ToDto(),
            urlHelper.CreateImageUrl(personProfile.ProfileImage),
            personProfile.Headline,
            personProfile.ShortPresentation,
            personProfile.Presentation,
            personProfile.ProfileVideo,
            personProfile.AvailableFromDate,
            personProfile.Email,
            personProfile.PhoneNumber);
    }

    public static CompetenceAreaDto ToDto(this Domain.Entities.CompetenceArea competenceArea)
    {
        return new (competenceArea.Id, competenceArea.Name);
    }

    public static OrganizationDto ToDto(this Domain.Entities.Organization organization)
    {
        // organization.Id, organization.Name, competenceArea.Created, competenceArea.CreatedBy?.ToDto(), competenceArea.LastModified, competenceArea.LastModifiedBy?.ToDto(

        return new (organization.Id, organization.Name, organization.Address?.ToDto());
    }

    public static AddressDto ToDto(this Domain.ValueObjects.Address address)
    {
        return new (address.Address1, address.Address2, address.PostalCode, address.Locality, address.SubAdminArea, address.AdminArea, address.Country);
    }

    public static ExperienceDto ToDto(this Domain.Entities.PersonProfileExperience experience)
    {
        return null!;
    }

    /*
    public static EmploymentDto ToDto(this Domain.Entities.Employment employment)
    {
        return new (employment.Id, employment.Employer.ToDto(), employment.Location, employment.EmploymentType.ToString(), employment.Description, employment.StartDate, employment.EndDate);
    }
    */

    public static EmploymentDto ToDto(this Domain.Entities.Employment employment, bool hierarchical = true)
    {
        return new EmploymentDto(
            employment.Id,
            employment.Employer.ToDto(), employment.Location, employment.EmploymentType,
            employment.StartDate, employment.EndDate, 
            employment.Description, 
            employment.Roles.Select(x => x.ToDto()),
            employment.Skills.OrderBy(s => s.PersonProfileSkill.Skill.Name).Select(x => x.PersonProfileSkill.ToDto()),
            hierarchical ? employment.Assignments.Select(a => a.ToDto()) : []);
    }

    public static EmploymentShortDto ToShortDto(this Domain.Entities.Employment employment)
    {
        return new EmploymentShortDto(
            employment.Id,
            employment.Employer.ToDto(), employment.Location, employment.EmploymentType,
            employment.StartDate, employment.EndDate);
    }

    public static AssignmentDto ToDto(this Domain.Entities.Assignment assignment)
    {
        return new AssignmentDto(
            assignment.Id,
            assignment.Employment.ToShortDto(), assignment.Company.ToDto(), assignment.Location, assignment.AssignmentType,
            assignment.StartDate, assignment.EndDate,
            assignment.Description,
            assignment.Roles.Select(x => x.ToDto()),
            assignment.Skills.OrderBy(s => s.PersonProfileSkill.Skill.Name).Select(x => x.PersonProfileSkill.ToDto()));
    }

    public static AssignmentShortDto ToShortDto(this Domain.Entities.Assignment assignment)
    {
        return new AssignmentShortDto(
            assignment.Id,
            assignment.Employment.ToShortDto(), assignment.Company.ToDto(), assignment.Location, assignment.AssignmentType,
            assignment.StartDate, assignment.EndDate);
    }

    public static RoleDto ToDto(this Domain.Entities.EmploymentRole role)
    {
        return new(role.Id, role.Title, role.Employment?.ToShortDto(), role.Assignment?.ToShortDto(), role.Location, role.StartDate, role.EndDate, role.Description, role.Skills.OrderBy(s => s.Skill.Name).Select(x => x.ToDto()));
    }

    public static SkillDto ToDto(this Domain.Entities.Skill skill)
    {
        return new (skill.Id, skill.Name, skill.Area.ToDto());
    }

    public static SkillAreaDto ToDto(this Domain.Entities.SkillArea skillArea)
    {
        return new (skillArea.Id, skillArea.Name, skillArea.Industry.ToDto());
    }

    public static PersonProfileSkillDto ToDto(this Domain.Entities.PersonProfileSkill personProfileSkill)
    {
        return new (personProfileSkill.Id, personProfileSkill.Skill.ToDto(), personProfileSkill.Level, personProfileSkill.Comment, personProfileSkill.Link?.ToDto());
    }

    public static LinkDto ToDto(this Domain.ValueObjects.Link link)
    {
        return new (link.Title, link.Href);
    }

    public static CaseDto ToDto(this Domain.Entities.Case @case, IUrlHelper urlHelper)
    {
        return new (@case.Id, @case.Status.ToString(), @case.Description, @case.CaseProfiles.Select(x => x.ToDto(urlHelper)),
            new CasePricingDto(@case.Pricing.HourlyPrice, @case.Pricing.Hours, @case.Pricing.Total));
    }

    public static CaseProfileDto ToDto(this Domain.Entities.CaseProfile caseProfile, IUrlHelper urlHelper)
    {
        return new (caseProfile.Id, caseProfile.PersonProfile.ToDto(urlHelper), caseProfile.Presentation);
    }

    public static CompanyDto ToDto(this Domain.Entities.Company company)
    {
        return new (company.Id, company.Name, company.Logo, company.Link, company.Industry.ToDto());
    }

    public static IndustryDto ToDto(this Domain.Entities.Industry industry)
    {
        return new (industry.Id, industry.Name);
    }
}