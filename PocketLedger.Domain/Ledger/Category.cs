using PocketLedger.Domain.Common.Primitives.GuidTypes;
using PocketLedger.Domain.Common.Primitives.StringTypes;

namespace PocketLedger.Domain.Ledger;

public sealed class Category
{
    public CategoryId Id { get; }
    public Name Name { get; private set; }
    public bool IsActive { get; private set; }
    public DateTimeOffset CreatedAt { get; }
    public DateTimeOffset UpdatedAt { get; private set; }

    private Category(
        CategoryId id,
        Name name,
        bool isActive,
        DateTimeOffset createdAt,
        DateTimeOffset updatedAt)
    {
        Id = id;
        Name = name;
        IsActive = isActive;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public static Category Create(
        Name name,
        DateTimeOffset createdAt)
    {
        return new Category(
            CategoryId.New(),
            name,
            true,
            createdAt,
            createdAt);
    }

    public void Activate(DateTimeOffset updatedAt)
    {
        if (IsActive)
        {
            return;
        }

        IsActive = true;
        UpdatedAt = updatedAt;
    }

    public void Deactivate(DateTimeOffset updatedAt)
    {
        if (!IsActive)
        {
            return;
        }

        IsActive = false;
        UpdatedAt = updatedAt;
    }

    public void UpdateName(Name name, DateTimeOffset updatedAt)
    {
        Name = name;
        UpdatedAt = updatedAt;
    }
}