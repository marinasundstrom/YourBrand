namespace Worker.Contracts;

public record SendEmail(string Recipient, string Subject, string Body);