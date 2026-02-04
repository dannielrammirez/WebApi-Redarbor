namespace Redarbor.Application.DTOs;

public sealed record UpdateEmployeeDto
{
    public int CompanyId { get; init; }
    public string? Email { get; init; }
    public string? Fax { get; init; }
    public string? Name { get; init; }
    public string? Password { get; init; }
    public int PortalId { get; init; }
    public int RoleId { get; init; }
    public int StatusId { get; init; }
    public string? Telephone { get; init; }
    public string? Username { get; init; }
}
