using System;

namespace Accounting.Application.Verifications;

public record VerificationsResult(IEnumerable<VerificationDto> Verifications, int TotalItems);