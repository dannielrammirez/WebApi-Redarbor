using Microsoft.Extensions.Logging;
using Redarbor.Application.Common;
using Redarbor.Domain.Exceptions;
using Redarbor.Domain.Interfaces;

namespace Redarbor.Application.Employees.Commands.DeleteEmployee;

public sealed class DeleteEmployeeCommandHandler : ICommandHandler<DeleteEmployeeCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteEmployeeCommandHandler> _logger;

    public DeleteEmployeeCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<DeleteEmployeeCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(request.Id, cancellationToken);

            if (employee is null)
            {
                _logger.LogWarning("Empleado con ID {EmployeeId} no encontrado para eliminación", request.Id);
                return Result.Failure($"Empleado con ID '{request.Id}' no fue encontrado.");
            }

            if (employee.IsDeleted)
            {
                _logger.LogWarning("Empleado con ID {EmployeeId} ya fue eliminado", request.Id);
                return Result.Failure($"El empleado con ID '{request.Id}' ya fue eliminado.");
            }

            employee.Delete();

            await _unitOfWork.Employees.UpdateAsync(employee, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Empleado con ID {EmployeeId} eliminado exitosamente", request.Id);

            return Result.Success();
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Validación de dominio fallida al eliminar empleado con ID {EmployeeId}", request.Id);
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al eliminar empleado con ID {EmployeeId}", request.Id);
            return Result.Failure("Ocurrió un error inesperado al eliminar el empleado.");
        }
    }
}
