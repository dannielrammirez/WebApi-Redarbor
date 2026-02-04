using Redarbor.Domain.Entities;

namespace Redarbor.Domain.Interfaces;

public interface IEmployeeReadRepository
{
    Task<IEnumerable<EmployeeReadModel>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<EmployeeReadModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<EmployeeReadModel>> GetByCompanyIdAsync(int companyId, CancellationToken cancellationToken = default);
    Task<IEnumerable<EmployeeReadModel>> GetByStatusIdAsync(int statusId, CancellationToken cancellationToken = default);
    Task<EmployeeReadModel?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
}

public sealed record EmployeeReadModel
{
    public int Id { get; init; }
    public int CompanyId { get; init; }
    public DateTime CreatedOn { get; init; }
    public DateTime? DeletedOn { get; init; }
    public string Email { get; init; } = string.Empty;
    public string? Fax { get; init; }
    public string? Name { get; init; }
    public DateTime? LastLogin { get; init; }
    public int PortalId { get; init; }
    public int RoleId { get; init; }
    public int StatusId { get; init; }
    public string? Telephone { get; init; }
    public DateTime? UpdatedOn { get; init; }
    public string Username { get; init; } = string.Empty;
}
