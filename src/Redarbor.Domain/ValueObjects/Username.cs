using System.Text.RegularExpressions;
using Redarbor.Domain.Common;
using Redarbor.Domain.Exceptions;

namespace Redarbor.Domain.ValueObjects;

public sealed partial class Username : ValueObject
{
    private static readonly Regex UsernameRegex = MyUsernameRegex();

    public string Value { get; }

    private Username(string value)
    {
        Value = value;
    }

    public static Username Create(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new DomainException("El nombre de usuario no puede estar vacío.");

        username = username.Trim();

        if (username.Length < 3)
            throw new DomainException("El nombre de usuario debe tener al menos 3 caracteres.");

        if (username.Length > 50)
            throw new DomainException("El nombre de usuario no puede exceder 50 caracteres.");

        if (!UsernameRegex.IsMatch(username))
            throw new DomainException("El nombre de usuario solo puede contener letras, números, guiones bajos y guiones.");

        return new Username(username);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(Username username) => username.Value;

    [GeneratedRegex(@"^[a-zA-Z0-9_-]+$", RegexOptions.Compiled)]
    private static partial Regex MyUsernameRegex();
}
