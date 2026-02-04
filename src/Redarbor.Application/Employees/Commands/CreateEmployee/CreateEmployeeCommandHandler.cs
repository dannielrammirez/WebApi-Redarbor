using Microsoft.Extensions.Logging;
using Redarbor.Application.Common;
using Redarbor.Application.DTOs;
using Redarbor.Domain.Entities;
using Redarbor.Domain.Exceptions;
using Redarbor.Domain.Interfaces;

namespace Redarbor.Application.Employees.Commands.CreateEmployee;

public sealed class CreateEmployeeCommandHandler : ICommandHandler<CreateEmployeeCommand, EmployeeDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateEmployeeCommandHandler> _logger;

    public CreateEmployeeCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<CreateEmployeeCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<EmployeeDto>> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var dto = request.Employee;

            if (await _unitOfWork.Employees.UsernameExistsAsync(dto.Username, cancellationToken: cancellationToken))
            {
                _logger.LogWarning("Intento de crear empleado con nombre de usuario duplicado: {Username}", dto.Username);
                return Result.Failure<EmployeeDto>($"El nombre de usuario '{dto.Username}' ya existe.");
            }

            if (await _unitOfWork.Employees.EmailExistsAsync(dto.Email, cancellationToken: cancellationToken))
            {
                _logger.LogWarning("Intento de crear empleado con correo electrónico duplicado: {Email}", dto.Email);
                return Result.Failure<EmployeeDto>($"El correo electrónico '{dto.Email}' ya existe.");
            }

            var employee = Employee.Create(
                companyId: dto.CompanyId,
                email: dto.Email,
                password: dto.Password,
                portalId: dto.PortalId,
                roleId: dto.RoleId,
                statusId: dto.StatusId,
                username: dto.Username,
                name: dto.Name,
                fax: dto.Fax,
                telephone: dto.Telephone);

            await _unitOfWork.Employees.AddAsync(employee, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Empleado creado exitosamente con ID: {EmployeeId}", employee.Id);

            var response = new EmployeeDto
            {
                Id = employee.Id,
                CompanyId = employee.CompanyId,
                CreatedOn = employee.CreatedOn,
                DeletedOn = employee.DeletedOn,
                Email = employee.Email.Value,
                Fax = employee.Fax,
                Name = employee.Name,
                LastLogin = employee.LastLogin,
                PortalId = employee.PortalId,
                RoleId = employee.RoleId,
                StatusId = employee.StatusId,
                Telephone = employee.Telephone,
                UpdatedOn = employee.UpdatedOn,
                Username = employee.Username.Value
            };

            return Result.Success(response);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Validación de dominio fallida al crear empleado");
            return Result.Failure<EmployeeDto>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al crear empleado");
            return Result.Failure<EmployeeDto>("Ocurrió un error inesperado al crear el empleado.");
        }
    }
}
