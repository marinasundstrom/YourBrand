using MassTransit;

namespace Documents.Contracts;

public record DocumentResponse(DocumentFormat DocumentFormat, MessageData<byte[]> Document);