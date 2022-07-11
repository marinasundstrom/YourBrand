namespace YourBrand.Notifications.Contracts;

public record SendEmail(string Recipient, string Subject, string Body);