using System.Security.Cryptography;
using Redarbor.Domain.Common;
using Redarbor.Domain.Exceptions;

namespace Redarbor.Domain.ValueObjects;

public sealed class HashedPassword : ValueObject
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 100000;
    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;

    public string Value { get; }

    private HashedPassword(string hashedValue)
    {
        Value = hashedValue;
    }

    public static HashedPassword Create(string plainPassword)
    {
        if (string.IsNullOrWhiteSpace(plainPassword))
            throw new DomainException("La contraseña no puede estar vacía.");

        if (plainPassword.Length < 6)
            throw new DomainException("La contraseña debe tener al menos 6 caracteres.");

        if (plainPassword.Length > 128)
            throw new DomainException("La contraseña no puede exceder 128 caracteres.");

        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(plainPassword, salt, Iterations, Algorithm, HashSize);

        var hashBytes = new byte[SaltSize + HashSize];
        Buffer.BlockCopy(salt, 0, hashBytes, 0, SaltSize);
        Buffer.BlockCopy(hash, 0, hashBytes, SaltSize, HashSize);

        return new HashedPassword(Convert.ToBase64String(hashBytes));
    }

    public static HashedPassword FromHash(string hashedValue)
    {
        if (string.IsNullOrWhiteSpace(hashedValue))
            throw new DomainException("La contraseña hash no puede estar vacía.");

        return new HashedPassword(hashedValue);
    }

    public bool Verify(string plainPassword)
    {
        if (string.IsNullOrWhiteSpace(plainPassword))
            return false;

        try
        {
            var hashBytes = Convert.FromBase64String(Value);

            if (hashBytes.Length != SaltSize + HashSize)
                return false;

            var salt = new byte[SaltSize];
            var storedHash = new byte[HashSize];

            Buffer.BlockCopy(hashBytes, 0, salt, 0, SaltSize);
            Buffer.BlockCopy(hashBytes, SaltSize, storedHash, 0, HashSize);

            var computedHash = Rfc2898DeriveBytes.Pbkdf2(plainPassword, salt, Iterations, Algorithm, HashSize);

            return CryptographicOperations.FixedTimeEquals(storedHash, computedHash);
        }
        catch
        {
            return false;
        }
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => "########";

    public static implicit operator string(HashedPassword password) => password.Value;
}
