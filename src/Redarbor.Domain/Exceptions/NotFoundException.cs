namespace Redarbor.Domain.Exceptions;

public class NotFoundException : Exception
{
    public string EntityName { get; }
    public object EntityId { get; }

    public NotFoundException(string entityName, object entityId)
        : base($"{entityName} con id '{entityId}' no fue encontrado.")
    {
        EntityName = entityName;
        EntityId = entityId;
    }

    public NotFoundException(string message) : base(message)
    {
        EntityName = string.Empty;
        EntityId = string.Empty;
    }
}
