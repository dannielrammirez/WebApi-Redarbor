using Redarbor.Domain.Common;
using Redarbor.Domain.Exceptions;
using Redarbor.Domain.ValueObjects;

namespace Redarbor.Domain.Entities;

public sealed class Employee : AuditableEntity<int>
{
    public int CompanyId { get; private set; }
    public Email Email { get; private set; } = null!;
    public string? Fax { get; private set; }
    public string? Name { get; private set; }
    public DateTime? LastLogin { get; private set; }
    public HashedPassword Password { get; private set; } = null!;
    public int PortalId { get; private set; }
    public int RoleId { get; private set; }
    public int StatusId { get; private set; }
    public string? Telephone { get; private set; }
    public Username Username { get; private set; } = null!;

    private Employee() : base() { }

    private Employee(
        int companyId,
        Email email,
        HashedPassword password,
        int portalId,
        int roleId,
        int statusId,
        Username username,
        string? name = null,
        string? fax = null,
        string? telephone = null) : base()
    {
        ValidateCompanyId(companyId);
        ValidatePortalId(portalId);
        ValidateRoleId(roleId);
        ValidateStatusId(statusId);

        CompanyId = companyId;
        Email = email;
        Password = password;
        PortalId = portalId;
        RoleId = roleId;
        StatusId = statusId;
        Username = username;
        Name = name?.Trim();
        Fax = fax?.Trim();
        Telephone = telephone?.Trim();
        SetCreatedOn(DateTime.UtcNow);
    }

    public static Employee Create(
        int companyId,
        string email,
        string password,
        int portalId,
        int roleId,
        int statusId,
        string username,
        string? name = null,
        string? fax = null,
        string? telephone = null)
    {
        var emailVO = Email.Create(email);
        var passwordVO = HashedPassword.Create(password);
        var usernameVO = Username.Create(username);

        return new Employee(
            companyId,
            emailVO,
            passwordVO,
            portalId,
            roleId,
            statusId,
            usernameVO,
            name,
            fax,
            telephone);
    }

    public static Employee Reconstitute(
        int id,
        int companyId,
        DateTime createdOn,
        DateTime? deletedOn,
        string email,
        string? fax,
        string? name,
        DateTime? lastLogin,
        string passwordHash,
        int portalId,
        int roleId,
        int statusId,
        string? telephone,
        DateTime? updatedOn,
        string username)
    {
        var employee = new Employee
        {
            Id = id,
            CompanyId = companyId,
            Email = Email.Create(email),
            Fax = fax,
            Name = name,
            LastLogin = lastLogin,
            Password = HashedPassword.FromHash(passwordHash),
            PortalId = portalId,
            RoleId = roleId,
            StatusId = statusId,
            Telephone = telephone,
            Username = Username.Create(username)
        };

        employee.SetCreatedOn(createdOn);
        if (updatedOn.HasValue)
            employee.SetUpdatedOn(updatedOn.Value);
        if (deletedOn.HasValue)
            employee.MarkAsDeleted(deletedOn.Value);

        return employee;
    }

    public void UpdateUsername(string newUsername)
    {
        Username = Username.Create(newUsername);
        SetUpdatedOn(DateTime.UtcNow);
    }

    public void UpdateEmail(string newEmail)
    {
        Email = Email.Create(newEmail);
        SetUpdatedOn(DateTime.UtcNow);
    }

    public void UpdatePassword(string newPassword)
    {
        Password = HashedPassword.Create(newPassword);
        SetUpdatedOn(DateTime.UtcNow);
    }

    public void UpdateContactInfo(string? name, string? telephone, string? fax)
    {
        Name = name?.Trim();
        Telephone = telephone?.Trim();
        Fax = fax?.Trim();
        SetUpdatedOn(DateTime.UtcNow);
    }

    public void UpdateCompanyId(int companyId)
    {
        ValidateCompanyId(companyId);
        CompanyId = companyId;
        SetUpdatedOn(DateTime.UtcNow);
    }

    public void UpdatePortalId(int portalId)
    {
        ValidatePortalId(portalId);
        PortalId = portalId;
        SetUpdatedOn(DateTime.UtcNow);
    }

    public void UpdateRoleId(int roleId)
    {
        ValidateRoleId(roleId);
        RoleId = roleId;
        SetUpdatedOn(DateTime.UtcNow);
    }

    public void UpdateStatusId(int statusId)
    {
        ValidateStatusId(statusId);
        StatusId = statusId;
        SetUpdatedOn(DateTime.UtcNow);
    }

    public void RecordLogin()
    {
        LastLogin = DateTime.UtcNow;
        SetUpdatedOn(DateTime.UtcNow);
    }

    public bool VerifyPassword(string plainPassword)
    {
        return Password.Verify(plainPassword);
    }

    public void Delete()
    {
        if (IsDeleted)
            throw new DomainException("El empleado ya fue eliminado.");

        MarkAsDeleted(DateTime.UtcNow);
    }

    private static void ValidateCompanyId(int companyId)
    {
        if (companyId <= 0)
            throw new DomainException("CompanyId debe ser mayor que cero.");
    }

    private static void ValidatePortalId(int portalId)
    {
        if (portalId <= 0)
            throw new DomainException("PortalId debe ser mayor que cero.");
    }

    private static void ValidateRoleId(int roleId)
    {
        if (roleId <= 0)
            throw new DomainException("RoleId debe ser mayor que cero.");
    }

    private static void ValidateStatusId(int statusId)
    {
        if (statusId <= 0)
            throw new DomainException("StatusId debe ser mayor que cero.");
    }
}
