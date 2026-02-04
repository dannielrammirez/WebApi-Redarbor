using Redarbor.Application.Common;
using Redarbor.Application.DTOs;

namespace Redarbor.Application.Employees.Commands.UpdateEmployee;

public sealed record UpdateEmployeeCommand : ICommand
{
    public int Id { get; init; }
    public required UpdateEmployeeDto Employee { get; init; }
}
