using Microsoft.Extensions.Logging;
using Redarbor.Application.Common;
using Redarbor.Domain.Exceptions;
using Redarbor.Domain.Interfaces;

namespace Redarbor.Application.Employees.Commands.UpdateEmployee;

public sealed class UpdateEmployeeCommandHandler : ICommandHandler<UpdateEmployeeCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateEmployeeCommandHandler> _logger;

    public UpdateEmployeeCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<UpdateEmployeeCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(request.Id, cancellationToken);

            if (employee is null)
            {
                _logger.LogWarning("Empleado con ID {EmployeeId} no encontrado para actualización", request.Id);
                return Result.Failure($"Empleado con ID '{request.Id}' no fue encontrado.");
            }

            if (employee.IsDeleted)
            {
                _logger.LogWarning("Intento de actualizar empleado eliminado con ID {EmployeeId}", request.Id);
                return Result.Failure($"El empleado con ID '{request.Id}' ha sido eliminado.");
            }

            var dto = request.Employee;

            if (!string.IsNullOrEmpty(dto.Username) &&
                dto.Username != employee.Username.Value &&
                await _unitOfWork.Employees.UsernameExistsAsync(dto.Username, request.Id, cancellationToken))
            {
                _logger.LogWarning("Intento de actualizar empleado con nombre de usuario duplicado: {Username}", dto.Username);
                return Result.Failure($"El nombre de usuario '{dto.Username}' ya existe.");
            }

            if (!string.IsNullOrEmpty(dto.Email) &&
                dto.Email != employee.Email.Value &&
                await _unitOfWork.Employees.EmailExistsAsync(dto.Email, request.Id, cancellationToken))
            {
                _logger.LogWarning("Intento de actualizar empleado con correo electrónico duplicado: {Email}", dto.Email);
                return Result.Failure($"El correo electrónico '{dto.Email}' ya existe.");
            }

            if (!string.IsNullOrEmpty(dto.Username))
                employee.UpdateUsername(dto.Username);

            if (!string.IsNullOrEmpty(dto.Email))
                employee.UpdateEmail(dto.Email);

            if (!string.IsNullOrEmpty(dto.Password))
                employee.UpdatePassword(dto.Password);

            if (dto.CompanyId > 0)
                employee.UpdateCompanyId(dto.CompanyId);

            if (dto.PortalId > 0)
                employee.UpdatePortalId(dto.PortalId);

            if (dto.RoleId > 0)
                employee.UpdateRoleId(dto.RoleId);

            if (dto.StatusId > 0)
                employee.UpdateStatusId(dto.StatusId);

            employee.UpdateContactInfo(dto.Name, dto.Telephone, dto.Fax);

            await _unitOfWork.Employees.UpdateAsync(employee, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Empleado con ID {EmployeeId} actualizado exitosamente", request.Id);

            return Result.Success();
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Validación de dominio fallida al actualizar empleado con ID {EmployeeId}", request.Id);
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al actualizar empleado con ID {EmployeeId}", request.Id);
            return Result.Failure("Ocurrió un error inesperado al actualizar el empleado.");
        }
    }
}
