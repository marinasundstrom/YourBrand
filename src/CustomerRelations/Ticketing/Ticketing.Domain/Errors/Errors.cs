namespace YourBrand.Ticketing.Domain;

public static partial class Errors
{
    public static class Ticketing
    {
        public static readonly Error IssueNotFound = new Error(nameof(IssueNotFound), "Issue not found", string.Empty);
    }

    public static class Projects
    {
        public static readonly Error ProjectNotFound = new Error(nameof(ProjectNotFound), "Project not found", string.Empty);

        public static readonly Error ProjectMemberNotFound = new Error(nameof(ProjectMemberNotFound), "Project member not found", string.Empty);
    }

    public static class Users
    {
        public static readonly Error UserNotFound = new Error(nameof(UserNotFound), "User not found", string.Empty);
    }

    public static class Organizations
    {
        public static readonly Error OrganizationNotFound = new Error(nameof(OrganizationNotFound), "Organization not found", string.Empty);
    }
}