namespace Redarbor.Domain.Common;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}
