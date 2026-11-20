namespace YourBrand.Accounting.Application.Journal;

public record VerificationResult(IEnumerable<VerificationDto> SupportingDocuments, int TotalItems);