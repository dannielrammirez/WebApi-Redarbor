using Microsoft.Extensions.Logging;
using Redarbor.Application.Common;
using Redarbor.Application.DTOs;
using Redarbor.Domain.Interfaces;

namespace Redarbor.Application.Employees.Queries.GetAllEmployees;

public sealed class GetAllEmployeesQueryHandler : IQueryHandler<GetAllEmployeesQuery, IEnumerable<EmployeeDto>>
{
    private readonly IEmployeeReadRepository _readRepository;
    private readonly ILogger<GetAllEmployeesQueryHandler> _logger;

    public GetAllEmployeesQueryHandler(
        IEmployeeReadRepository readRepository,
        ILogger<GetAllEmployeesQueryHandler> logger)
    {
        _readRepository = readRepository;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<EmployeeDto>>> Handle(GetAllEmployeesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var employees = await _readRepository.GetAllAsync(cancellationToken);

            var dtos = employees.Select(e => new EmployeeDto
            {
                Id = e.Id,
                CompanyId = e.CompanyId,
                CreatedOn = e.CreatedOn,
                DeletedOn = e.DeletedOn,
                Email = e.Email,
                Fax = e.Fax,
                Name = e.Name,
                LastLogin = e.LastLogin,
                PortalId = e.PortalId,
                RoleId = e.RoleId,
                StatusId = e.StatusId,
                Telephone = e.Telephone,
                UpdatedOn = e.UpdatedOn,
                Username = e.Username
            }).ToList();

            _logger.LogDebug("Se obtuvieron {Count} empleados", dtos.Count);

            return Result.Success<IEnumerable<EmployeeDto>>(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al obtener todos los empleados");
            return Result.Failure<IEnumerable<EmployeeDto>>("Ocurrió un error inesperado al obtener los empleados.");
        }
    }
}
