namespace Redarbor.Application.DTOs;

public sealed record CreateEmployeeDto
{
    public int CompanyId { get; init; }
    public required string Email { get; init; }
    public string? Fax { get; init; }
    public string? Name { get; init; }
    public required string Password { get; init; }
    public int PortalId { get; init; }
    public int RoleId { get; init; }
    public int StatusId { get; init; }
    public string? Telephone { get; init; }
    public required string Username { get; init; }
}
