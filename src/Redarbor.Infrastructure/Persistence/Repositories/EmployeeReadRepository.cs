using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Redarbor.Domain.Interfaces;

namespace Redarbor.Infrastructure.Persistence.Repositories;

public sealed class EmployeeReadRepository : IEmployeeReadRepository
{
    private readonly string _connectionString;

    public EmployeeReadRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Cadena de conexión 'DefaultConnection' no encontrada.");
    }

    private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

    public async Task<IEnumerable<EmployeeReadModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT
                Id,
                CompanyId,
                CreatedOn,
                DeletedOn,
                Email,
                Fax,
                Name,
                LastLogin,
                PortalId,
                RoleId,
                StatusId,
                Telephone,
                UpdatedOn,
                Username
            FROM Employees
            WHERE DeletedOn IS NULL
            ORDER BY CreatedOn DESC
            """;

        using var connection = CreateConnection();
        var command = new CommandDefinition(sql, cancellationToken: cancellationToken);
        return await connection.QueryAsync<EmployeeReadModel>(command);
    }

    public async Task<EmployeeReadModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT
                Id,
                CompanyId,
                CreatedOn,
                DeletedOn,
                Email,
                Fax,
                Name,
                LastLogin,
                PortalId,
                RoleId,
                StatusId,
                Telephone,
                UpdatedOn,
                Username
            FROM Employees
            WHERE Id = @Id AND DeletedOn IS NULL
            """;

        using var connection = CreateConnection();
        var command = new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken);
        return await connection.QueryFirstOrDefaultAsync<EmployeeReadModel>(command);
    }

    public async Task<IEnumerable<EmployeeReadModel>> GetByCompanyIdAsync(int companyId, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT
                Id,
                CompanyId,
                CreatedOn,
                DeletedOn,
                Email,
                Fax,
                Name,
                LastLogin,
                PortalId,
                RoleId,
                StatusId,
                Telephone,
                UpdatedOn,
                Username
            FROM Employees
            WHERE CompanyId = @CompanyId AND DeletedOn IS NULL
            ORDER BY CreatedOn DESC
            """;

        using var connection = CreateConnection();
        var command = new CommandDefinition(sql, new { CompanyId = companyId }, cancellationToken: cancellationToken);
        return await connection.QueryAsync<EmployeeReadModel>(command);
    }

    public async Task<IEnumerable<EmployeeReadModel>> GetByStatusIdAsync(int statusId, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT
                Id,
                CompanyId,
                CreatedOn,
                DeletedOn,
                Email,
                Fax,
                Name,
                LastLogin,
                PortalId,
                RoleId,
                StatusId,
                Telephone,
                UpdatedOn,
                Username
            FROM Employees
            WHERE StatusId = @StatusId AND DeletedOn IS NULL
            ORDER BY CreatedOn DESC
            """;

        using var connection = CreateConnection();
        var command = new CommandDefinition(sql, new { StatusId = statusId }, cancellationToken: cancellationToken);
        return await connection.QueryAsync<EmployeeReadModel>(command);
    }

    public async Task<EmployeeReadModel?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT
                Id,
                CompanyId,
                CreatedOn,
                DeletedOn,
                Email,
                Fax,
                Name,
                LastLogin,
                PortalId,
                RoleId,
                StatusId,
                Telephone,
                UpdatedOn,
                Username
            FROM Employees
            WHERE Username = @Username AND DeletedOn IS NULL
            """;

        using var connection = CreateConnection();
        var command = new CommandDefinition(sql, new { Username = username }, cancellationToken: cancellationToken);
        return await connection.QueryFirstOrDefaultAsync<EmployeeReadModel>(command);
    }
}
