namespace Documents.Contracts;

public record DocumentType(string Id, string Name);

public record DocumentCreated(string Id, string Name, DocumentType DocumentType);

public record DocumentRenamed(string Id, string Name, DocumentType DocumentType);

public record DocumentDeleted(string Id, string Name, DocumentType DocumentType);