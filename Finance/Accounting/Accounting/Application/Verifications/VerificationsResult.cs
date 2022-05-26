using System;

namespace YourBrand.Accounting.Application.Verifications;

public record VerificationsResult(IEnumerable<VerificationDto> Verifications, int TotalItems);