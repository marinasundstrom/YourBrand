namespace YourBrand.Showroom.Domain.Entities;

public enum EmploymentType
{
    /// <summary>
    /// A permanent, full-time position with standard working hours and benefits.
    /// </summary>
    FullTime,

    /// <summary>
    /// A position with reduced working hours compared to full-time, often with prorated benefits.
    /// </summary>
    PartTime,

    /// <summary>
    /// A short-term position that lasts for a limited duration, often used to cover specific needs.
    /// </summary>
    Temporary,

    /// <summary>
    /// A fixed-term employment agreement, often project-based, without long-term commitment.
    /// </summary>
    Contract,

    /// <summary>
    /// A temporary work experience opportunity, often for students or recent graduates.
    /// </summary>
    Internship,

    /// <summary>
    /// Independent work where an individual provides services to multiple clients on a flexible basis.
    /// </summary>
    Freelance,

    /// <summary>
    /// Running one's own business or working independently rather than being employed by a company.
    /// </summary>
    SelfEmployment,

    /// <summary>
    /// A structured program where an individual learns a trade or profession under supervision.
    /// </summary>
    Apprenticeship,

    /// <summary>
    /// Unpaid or minimally paid work done for charitable, community, or nonprofit organizations.
    /// </summary>
    VolunteerWork,

    /// <summary>
    /// A work arrangement where the employee works entirely or primarily from a remote location.
    /// </summary>
    Remote,

    /// <summary>
    /// Employment that is based on a specific season or time period, such as holiday retail jobs.
    /// </summary>
    Seasonal,

    /// <summary>
    /// Employment with no guaranteed hours, where work is provided as needed.
    /// </summary>
    Casual,

    /// <summary>
    /// Work that is performed on an as-needed basis, typically with irregular hours.
    /// </summary>
    OnCall
}