using Redarbor.Application.Common;
using Redarbor.Application.DTOs;

namespace Redarbor.Application.Employees.Queries.GetEmployeeById;

public sealed record GetEmployeeByIdQuery : IQuery<EmployeeDto>
{
    public int Id { get; init; }
}
