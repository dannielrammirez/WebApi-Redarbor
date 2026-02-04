using MediatR;
using Microsoft.AspNetCore.Mvc;
using Redarbor.Application.DTOs;
using Redarbor.Application.Employees.Commands.CreateEmployee;
using Redarbor.Application.Employees.Commands.DeleteEmployee;
using Redarbor.Application.Employees.Commands.UpdateEmployee;
using Redarbor.Application.Employees.Queries.GetAllEmployees;
using Redarbor.Application.Employees.Queries.GetEmployeeById;

namespace Redarbor.API.Controllers;

[ApiController]
[Route("api/redarbor")]
[Produces("application/json")]
public class RedarborController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<RedarborController> _logger;

    public RedarborController(IMediator mediator, ILogger<RedarborController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<EmployeeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Solicitud para obtener todos los empleados");

        var result = await _mediator.Send(new GetAllEmployeesQuery(), cancellationToken);

        if (result.IsFailure)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Error",
                Detail = result.Error,
                Status = StatusCodes.Status500InternalServerError
            });
        }

        return Ok(result.Value);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Solicitud para obtener empleado con ID {EmployeeId}", id);

        var result = await _mediator.Send(new GetEmployeeByIdQuery { Id = id }, cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error?.Contains("no fue encontrado") == true)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "No encontrado",
                    Detail = result.Error,
                    Status = StatusCodes.Status404NotFound
                });
            }

            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Error",
                Detail = result.Error,
                Status = StatusCodes.Status500InternalServerError
            });
        }

        return Ok(result.Value);
    }

    [HttpPost]
    [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CreateEmployeeDto request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Solicitud para crear nuevo empleado con nombre de usuario {Username}", request.Username);

        var result = await _mediator.Send(new CreateEmployeeCommand { Employee = request }, cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error?.Contains("ya existe") == true)
            {
                return Conflict(new ProblemDetails
                {
                    Title = "Conflicto",
                    Detail = result.Error,
                    Status = StatusCodes.Status409Conflict
                });
            }

            if (result.Errors.Count > 0)
            {
                return BadRequest(new ValidationProblemDetails
                {
                    Title = "Error de validación",
                    Detail = "Uno o más errores de validación ocurrieron.",
                    Status = StatusCodes.Status400BadRequest,
                    Errors = { { "Employee", result.Errors.ToArray() } }
                });
            }

            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Error",
                Detail = result.Error,
                Status = StatusCodes.Status500InternalServerError
            });
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateEmployeeDto request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Solicitud para actualizar empleado con ID {EmployeeId}", id);

        var result = await _mediator.Send(new UpdateEmployeeCommand { Id = id, Employee = request }, cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error?.Contains("no fue encontrado") == true)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "No encontrado",
                    Detail = result.Error,
                    Status = StatusCodes.Status404NotFound
                });
            }

            if (result.Error?.Contains("ya existe") == true)
            {
                return Conflict(new ProblemDetails
                {
                    Title = "Conflicto",
                    Detail = result.Error,
                    Status = StatusCodes.Status409Conflict
                });
            }

            if (result.Errors.Count > 0)
            {
                return BadRequest(new ValidationProblemDetails
                {
                    Title = "Error de validación",
                    Detail = "Uno o más errores de validación ocurrieron.",
                    Status = StatusCodes.Status400BadRequest,
                    Errors = { { "Employee", result.Errors.ToArray() } }
                });
            }

            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Error",
                Detail = result.Error,
                Status = StatusCodes.Status500InternalServerError
            });
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Solicitud para eliminar empleado con ID {EmployeeId}", id);

        var result = await _mediator.Send(new DeleteEmployeeCommand { Id = id }, cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error?.Contains("no fue encontrado") == true || result.Error?.Contains("ya fue eliminado") == true)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "No encontrado",
                    Detail = result.Error,
                    Status = StatusCodes.Status404NotFound
                });
            }

            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Error",
                Detail = result.Error,
                Status = StatusCodes.Status500InternalServerError
            });
        }

        return NoContent();
    }
}
