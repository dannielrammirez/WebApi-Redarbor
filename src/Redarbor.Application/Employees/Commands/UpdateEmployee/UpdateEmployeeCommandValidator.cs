using FluentValidation;

namespace Redarbor.Application.Employees.Commands.UpdateEmployee;

public sealed class UpdateEmployeeCommandValidator : AbstractValidator<UpdateEmployeeCommand>
{
    public UpdateEmployeeCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("El ID del empleado debe ser mayor que cero.");

        RuleFor(x => x.Employee)
            .NotNull()
            .WithMessage("Los datos del empleado son obligatorios.");

        When(x => x.Employee != null, () =>
        {
            RuleFor(x => x.Employee.Email)
                .MaximumLength(256)
                .WithMessage("El correo electrónico no puede exceder 256 caracteres.")
                .EmailAddress()
                .WithMessage("Formato de correo electrónico inválido.")
                .When(x => !string.IsNullOrEmpty(x.Employee.Email));

            RuleFor(x => x.Employee.Password)
                .MinimumLength(6)
                .WithMessage("La contraseña debe tener al menos 6 caracteres.")
                .MaximumLength(128)
                .WithMessage("La contraseña no puede exceder 128 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.Employee.Password));

            RuleFor(x => x.Employee.Username)
                .MinimumLength(3)
                .WithMessage("El nombre de usuario debe tener al menos 3 caracteres.")
                .MaximumLength(50)
                .WithMessage("El nombre de usuario no puede exceder 50 caracteres.")
                .Matches(@"^[a-zA-Z0-9_-]+$")
                .WithMessage("El nombre de usuario solo puede contener letras, números, guiones bajos y guiones.")
                .When(x => !string.IsNullOrEmpty(x.Employee.Username));

            RuleFor(x => x.Employee.Name)
                .MaximumLength(100)
                .WithMessage("El nombre no puede exceder 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.Employee.Name));

            RuleFor(x => x.Employee.Telephone)
                .MaximumLength(20)
                .WithMessage("El teléfono no puede exceder 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.Employee.Telephone));

            RuleFor(x => x.Employee.Fax)
                .MaximumLength(20)
                .WithMessage("El fax no puede exceder 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.Employee.Fax));
        });
    }
}
