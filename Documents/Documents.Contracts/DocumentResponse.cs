using MassTransit;

namespace YourBrand.Documents.Contracts;

public record DocumentResponse(DocumentFormat DocumentFormat, MessageData<byte[]> Document);