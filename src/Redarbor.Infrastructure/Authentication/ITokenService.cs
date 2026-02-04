namespace Redarbor.Infrastructure.Authentication;

public interface ITokenService
{
    string GenerateAccessToken(int employeeId, string username, string email);
}
