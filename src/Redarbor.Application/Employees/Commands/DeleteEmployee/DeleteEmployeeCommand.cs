using Redarbor.Application.Common;

namespace Redarbor.Application.Employees.Commands.DeleteEmployee;

public sealed record DeleteEmployeeCommand : ICommand
{
    public int Id { get; init; }
}
