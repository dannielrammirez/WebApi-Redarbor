namespace Redarbor.Domain.Common;

public abstract class AuditableEntity<TId> : Entity<TId> where TId : notnull
{
    public DateTime CreatedOn { get; protected set; }
    public DateTime? UpdatedOn { get; protected set; }
    public DateTime? DeletedOn { get; protected set; }

    protected AuditableEntity() : base() { }

    protected AuditableEntity(TId id) : base(id) { }

    public void SetCreatedOn(DateTime createdOn)
    {
        CreatedOn = createdOn;
    }

    public void SetUpdatedOn(DateTime updatedOn)
    {
        UpdatedOn = updatedOn;
    }

    public void MarkAsDeleted(DateTime deletedOn)
    {
        DeletedOn = deletedOn;
    }

    public bool IsDeleted => DeletedOn.HasValue;
}
