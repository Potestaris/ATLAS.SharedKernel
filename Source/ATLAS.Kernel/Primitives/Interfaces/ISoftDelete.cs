namespace ATLAS.Kernel.Primitives.Interfaces;

/// <summary>
/// Marks an entity that supports soft deletion.
/// Entities implementing this interface set a flag to mark deletion rather than
/// being physically removed from the database.
/// </summary>
public interface ISoftDeletable
{
    /// <summary>
    /// Gets a value indicating whether this entity has been marked as deleted.
    /// </summary>
    bool Erased { get; set; }
}
