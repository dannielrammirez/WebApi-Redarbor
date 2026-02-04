using Redarbor.Application.Common;
using Redarbor.Application.DTOs;

namespace Redarbor.Application.Employees.Queries.GetAllEmployees;

public sealed record GetAllEmployeesQuery : IQuery<IEnumerable<EmployeeDto>>
{
}
