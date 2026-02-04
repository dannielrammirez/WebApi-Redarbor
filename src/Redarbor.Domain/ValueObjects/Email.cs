using System.Text.RegularExpressions;
using Redarbor.Domain.Common;
using Redarbor.Domain.Exceptions;

namespace Redarbor.Domain.ValueObjects;

public sealed partial class Email : ValueObject
{
    private static readonly Regex EmailRegex = MyEmailRegex();

    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    public static Email Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("El correo electrónico no puede estar vacío.");

        email = email.Trim().ToLowerInvariant();

        if (email.Length > 256)
            throw new DomainException("El correo electrónico no puede exceder 256 caracteres.");

        if (!EmailRegex.IsMatch(email))
            throw new DomainException("Formato de correo electrónico inválido.");

        return new Email(email);
    }

    public static Email? CreateOptional(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return null;

        return Create(email);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(Email email) => email.Value;

    [GeneratedRegex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", RegexOptions.Compiled)]
    private static partial Regex MyEmailRegex();
}
