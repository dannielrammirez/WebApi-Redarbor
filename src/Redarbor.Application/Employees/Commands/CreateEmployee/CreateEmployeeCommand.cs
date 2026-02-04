using Redarbor.Application.Common;
using Redarbor.Application.DTOs;

namespace Redarbor.Application.Employees.Commands.CreateEmployee;

public sealed record CreateEmployeeCommand : ICommand<EmployeeDto>
{
    public required CreateEmployeeDto Employee { get; init; }
}
