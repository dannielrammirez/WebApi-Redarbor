using Microsoft.Extensions.Logging;
using Redarbor.Application.Common;
using Redarbor.Application.DTOs;
using Redarbor.Domain.Interfaces;

namespace Redarbor.Application.Employees.Queries.GetEmployeeById;

public sealed class GetEmployeeByIdQueryHandler : IQueryHandler<GetEmployeeByIdQuery, EmployeeDto>
{
    private readonly IEmployeeReadRepository _readRepository;
    private readonly ILogger<GetEmployeeByIdQueryHandler> _logger;

    public GetEmployeeByIdQueryHandler(
        IEmployeeReadRepository readRepository,
        ILogger<GetEmployeeByIdQueryHandler> logger)
    {
        _readRepository = readRepository;
        _logger = logger;
    }

    public async Task<Result<EmployeeDto>> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var employee = await _readRepository.GetByIdAsync(request.Id, cancellationToken);

            if (employee is null)
            {
                _logger.LogWarning("Empleado con ID {EmployeeId} no encontrado", request.Id);
                return Result.Failure<EmployeeDto>($"Empleado con ID '{request.Id}' no fue encontrado.");
            }

            var dto = new EmployeeDto
            {
                Id = employee.Id,
                CompanyId = employee.CompanyId,
                CreatedOn = employee.CreatedOn,
                DeletedOn = employee.DeletedOn,
                Email = employee.Email,
                Fax = employee.Fax,
                Name = employee.Name,
                LastLogin = employee.LastLogin,
                PortalId = employee.PortalId,
                RoleId = employee.RoleId,
                StatusId = employee.StatusId,
                Telephone = employee.Telephone,
                UpdatedOn = employee.UpdatedOn,
                Username = employee.Username
            };

            _logger.LogDebug("Se obtuvo empleado con ID {EmployeeId}", request.Id);

            return Result.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al obtener empleado con ID {EmployeeId}", request.Id);
            return Result.Failure<EmployeeDto>("Ocurrió un error inesperado al obtener el empleado.");
        }
    }
}
