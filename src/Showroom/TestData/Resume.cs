using System;
using System.Collections.Generic;

using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace YourBrand.Showroom.TestData
{
    public partial class Resume
    {
        [JsonPropertyName("experience")]
        public Experience[] Experience { get; set; } = null!;

        [JsonPropertyName("education")]
        public Education[] Education { get; set; } = null!;
    }

    public partial class Experience
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }  = null!;

        [JsonPropertyName("employer")]
        public string Employer { get; set; }  = null!;

        [JsonPropertyName("employmentType")]
        public string EmploymentType { get; set; }  = null!;

        [JsonPropertyName("highlight")]
        public bool Highlight { get; set; }

        [JsonPropertyName("company")]
        public string Company { get; set; }  = null!;

        [JsonPropertyName("companyLogo")]
        public string? CompanyLogo { get; set; }  = null!;

        [JsonPropertyName("industry")]
        public string Industry { get; set; }  = null!;

        [JsonPropertyName("link")]
        public string? Link { get; set; }  = null!;

        [JsonPropertyName("location")]
        public string Location { get; set; }  = null!;

        [JsonPropertyName("current")]
        public bool Current { get; set; }

        [JsonPropertyName("startDate")]
        public DateTime StartDate { get; set; }

        [JsonPropertyName("endDate")]
        public DateTime? EndDate { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; } = null!;

        [JsonPropertyName("skills")]
        public string[] Skills { get; set; } = null!;
    }

    public partial class Education
    {
        [JsonPropertyName("school")]
        public string School { get; set; } = null!;

        [JsonPropertyName("logo")]
        public string? Logo { get; set; }  = null!;

        [JsonPropertyName("location")]
        public string Location { get; set; } = null!;

        [JsonPropertyName("degree")]
        public string? Degree { get; set; }

        [JsonPropertyName("fieldOfStudy")]
        public string FieldOfStudy { get; set; } = null!;

        [JsonPropertyName("startDate")]
        public DateTime StartDate { get; set; }

        [JsonPropertyName("endDate")]
        public DateTime? EndDate { get; set; }

        [JsonPropertyName("activities")]
        public string[] Activities { get; set; } = null!;

        [JsonPropertyName("description")]
        public string Description { get; set; } = null!;

        [JsonPropertyName("courses")]
        public string[] Courses { get; set; } = null!;

        [JsonPropertyName("link")]
        public string? Link { get; set; }  = null!;
    }

    public partial class Resume
    {
        public static Resume FromJson(string json) => JsonSerializer.Deserialize<Resume>(json)!;
    }

    public static class Serialize
    {
        public static string ToJson(this Resume self) => JsonSerializer.Serialize(self);
    }

    public static class GroupingExtensions 
    {
        public static void Deconstruct<TKey, TValue>(this IGrouping<TKey, TValue> source, out TKey key, out IEnumerable<TValue> items) 
        {
            key = source.Key;
            items = source.AsEnumerable();
        }
    }

    public static class ExperienceExtensions 
    {
        public static DateTime GetNowDate()
        {
           return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
        }

        public static DateTime GetStartDate(this Experience experience)
        {
            return new DateTime(experience.StartDate.Year, experience.StartDate.Month,  1);
        }

        public static DateTime? GetEndDate(this Experience experience)
        {
            DateTime? endDate = null;

            if(experience.EndDate != null) 
            {
                var _endDate = experience.EndDate.GetValueOrDefault();
                endDate = new DateTime(_endDate.Year, _endDate.Month,  DateTime.DaysInMonth(_endDate.Year, _endDate.Month));
            }

            return endDate;     
        }
    }

    public static class EducationExtensions 
    {
        public static DateTime GetNowDate()
        {
           return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
        }

        public static DateTime GetStartDate(this Education education)
        {
            return new DateTime(education.StartDate.Year, education.StartDate.Month,  1);
        }

        public static DateTime? GetEndDate(this Education education)
        {
            DateTime? endDate = null;

            if(education.EndDate != null) 
            {
                var _endDate = education.EndDate.GetValueOrDefault();
                endDate = new DateTime(_endDate.Year, _endDate.Month,  DateTime.DaysInMonth(_endDate.Year, _endDate.Month));
            }

            return endDate;     
        }
    }
}