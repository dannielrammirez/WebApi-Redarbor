namespace Redarbor.Application.DTOs;

public sealed record EmployeeDto
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
