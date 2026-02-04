using FluentValidation;

namespace Redarbor.Application.Employees.Commands.CreateEmployee;

public sealed class CreateEmployeeCommandValidator : AbstractValidator<CreateEmployeeCommand>
{
    public CreateEmployeeCommandValidator()
    {
        RuleFor(x => x.Employee)
            .NotNull()
            .WithMessage("Los datos del empleado son obligatorios.");

        When(x => x.Employee != null, () =>
        {
            RuleFor(x => x.Employee.CompanyId)
                .GreaterThan(0)
                .WithMessage("CompanyId debe ser mayor que cero.");

            RuleFor(x => x.Employee.Email)
                .NotEmpty()
                .WithMessage("El correo electrónico es obligatorio.")
                .MaximumLength(256)
                .WithMessage("El correo electrónico no puede exceder 256 caracteres.")
                .EmailAddress()
                .WithMessage("Formato de correo electrónico inválido.");

            RuleFor(x => x.Employee.Password)
                .NotEmpty()
                .WithMessage("La contraseña es obligatoria.")
                .MinimumLength(6)
                .WithMessage("La contraseña debe tener al menos 6 caracteres.")
                .MaximumLength(128)
                .WithMessage("La contraseña no puede exceder 128 caracteres.");

            RuleFor(x => x.Employee.PortalId)
                .GreaterThan(0)
                .WithMessage("PortalId debe ser mayor que cero.");

            RuleFor(x => x.Employee.RoleId)
                .GreaterThan(0)
                .WithMessage("RoleId debe ser mayor que cero.");

            RuleFor(x => x.Employee.StatusId)
                .GreaterThan(0)
                .WithMessage("StatusId debe ser mayor que cero.");

            RuleFor(x => x.Employee.Username)
                .NotEmpty()
                .WithMessage("El nombre de usuario es obligatorio.")
                .MinimumLength(3)
                .WithMessage("El nombre de usuario debe tener al menos 3 caracteres.")
                .MaximumLength(50)
                .WithMessage("El nombre de usuario no puede exceder 50 caracteres.")
                .Matches(@"^[a-zA-Z0-9_-]+$")
                .WithMessage("El nombre de usuario solo puede contener letras, números, guiones bajos y guiones.");

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
