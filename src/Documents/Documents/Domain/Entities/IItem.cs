namespace YourBrand.Documents.Domain.Entities;

public interface IItem
{
    string Id { get; }
    Directory? Directory { get; }
    string Name { get; }
    string? Description { get; }

    bool Rename(string newName);
    void UpdateDescription(string? description);
}