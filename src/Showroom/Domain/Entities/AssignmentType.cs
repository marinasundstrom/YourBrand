namespace YourBrand.Showroom.Domain.Entities;

public enum AssignmentType
{
    /// <summary>
    /// A brief assignment that lasts only a short period, typically a few weeks or months.
    /// </summary>
    ShortTerm,

    /// <summary>
    /// A prolonged assignment that extends over a long period within the employment.
    /// </summary>
    LongTerm,

    /// <summary>
    /// An assignment tied to a specific project, usually with defined deliverables.
    /// </summary>
    ProjectBased,

    /// <summary>
    /// A consulting-based assignment where expertise is provided to clients or internal teams.
    /// </summary>
    Consulting,

    /// <summary>
    /// An assignment focused on skill-building, training, or educational development.
    /// </summary>
    Training,

    /// <summary>
    /// A leadership role within an assignment, involving management, strategy, or oversight responsibilities.
    /// </summary>
    Leadership,

    /// <summary>
    /// An assignment focused on research, investigation, or exploratory work.
    /// </summary>
    Research,

    /// <summary>
    /// A task or series of tasks that are part of a contractual agreement.
    /// </summary>
    ContractedTask,

    /// <summary>
    /// An assignment that requires direct interaction with clients, such as consulting or service delivery.
    /// </summary>
    ClientEngagement,

    /// <summary>
    /// A temporary internal role change, such as a job rotation within an organization.
    /// </summary>
    InternalRotation,

    /// <summary>
    /// A mentoring-based assignment where an individual provides guidance or support to others.
    /// </summary>
    Mentoring,

    /// <summary>
    /// A role focused on providing support, assistance, or auxiliary functions within the organization.
    /// </summary>
    SupportRole,

    /// <summary>
    /// An assignment designed for personal or professional development and growth.
    /// </summary>
    Developmental,

    /// <summary>
    /// A remote-specific assignment where work is conducted from a non-office location.
    /// </summary>
    RemoteAssignment,

    /// <summary>
    /// A temporary assignment to cover an absent employee or fulfill a short-term need.
    /// </summary>
    TemporaryCover
}