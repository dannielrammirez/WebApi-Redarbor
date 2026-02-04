using FluentAssertions;
using Redarbor.Domain.Entities;
using Redarbor.Domain.Exceptions;
using Xunit;

namespace Redarbor.Domain.Tests.Entities;

public class EmployeeTests
{
    private const int ValidCompanyId = 1;
    private const string ValidEmail = "test@example.com";
    private const string ValidPassword = "SecurePassword123";
    private const int ValidPortalId = 1;
    private const int ValidRoleId = 1;
    private const int ValidStatusId = 1;
    private const string ValidUsername = "testuser";

    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        // Act
        var employee = Employee.Create(
            companyId: ValidCompanyId,
            email: ValidEmail,
            password: ValidPassword,
            portalId: ValidPortalId,
            roleId: ValidRoleId,
            statusId: ValidStatusId,
            username: ValidUsername,
            name: "John Doe",
            fax: "123456",
            telephone: "789012");

        // Assert
        employee.Should().NotBeNull();
        employee.CompanyId.Should().Be(ValidCompanyId);
        employee.Email.Value.Should().Be(ValidEmail);
        employee.PortalId.Should().Be(ValidPortalId);
        employee.RoleId.Should().Be(ValidRoleId);
        employee.StatusId.Should().Be(ValidStatusId);
        employee.Username.Value.Should().Be(ValidUsername);
        employee.Name.Should().Be("John Doe");
        employee.Fax.Should().Be("123456");
        employee.Telephone.Should().Be("789012");
        employee.CreatedOn.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        employee.UpdatedOn.Should().BeNull();
        employee.DeletedOn.Should().BeNull();
        employee.LastLogin.Should().BeNull();
        employee.IsDeleted.Should().BeFalse();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_WithInvalidCompanyId_ShouldThrowDomainException(int companyId)
    {
        // Act
        var act = () => Employee.Create(
            companyId: companyId,
            email: ValidEmail,
            password: ValidPassword,
            portalId: ValidPortalId,
            roleId: ValidRoleId,
            statusId: ValidStatusId,
            username: ValidUsername);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("CompanyId debe ser mayor que cero.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_WithInvalidPortalId_ShouldThrowDomainException(int portalId)
    {
        // Act
        var act = () => Employee.Create(
            companyId: ValidCompanyId,
            email: ValidEmail,
            password: ValidPassword,
            portalId: portalId,
            roleId: ValidRoleId,
            statusId: ValidStatusId,
            username: ValidUsername);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("PortalId debe ser mayor que cero.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_WithInvalidRoleId_ShouldThrowDomainException(int roleId)
    {
        // Act
        var act = () => Employee.Create(
            companyId: ValidCompanyId,
            email: ValidEmail,
            password: ValidPassword,
            portalId: ValidPortalId,
            roleId: roleId,
            statusId: ValidStatusId,
            username: ValidUsername);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("RoleId debe ser mayor que cero.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_WithInvalidStatusId_ShouldThrowDomainException(int statusId)
    {
        // Act
        var act = () => Employee.Create(
            companyId: ValidCompanyId,
            email: ValidEmail,
            password: ValidPassword,
            portalId: ValidPortalId,
            roleId: ValidRoleId,
            statusId: statusId,
            username: ValidUsername);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("StatusId debe ser mayor que cero.");
    }

    [Fact]
    public void UpdateUsername_ShouldUpdateUsernameAndTimestamp()
    {
        // Arrange
        var employee = CreateValidEmployee();
        var newUsername = "newusername";

        // Act
        employee.UpdateUsername(newUsername);

        // Assert
        employee.Username.Value.Should().Be(newUsername);
        employee.UpdatedOn.Should().NotBeNull();
        employee.UpdatedOn.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void UpdateEmail_ShouldUpdateEmailAndTimestamp()
    {
        // Arrange
        var employee = CreateValidEmployee();
        var newEmail = "newemail@example.com";

        // Act
        employee.UpdateEmail(newEmail);

        // Assert
        employee.Email.Value.Should().Be(newEmail);
        employee.UpdatedOn.Should().NotBeNull();
    }

    [Fact]
    public void UpdatePassword_ShouldUpdatePasswordAndTimestamp()
    {
        // Arrange
        var employee = CreateValidEmployee();
        var newPassword = "NewSecurePassword456";

        // Act
        employee.UpdatePassword(newPassword);

        // Assert
        employee.VerifyPassword(newPassword).Should().BeTrue();
        employee.VerifyPassword(ValidPassword).Should().BeFalse();
        employee.UpdatedOn.Should().NotBeNull();
    }

    [Fact]
    public void UpdateContactInfo_ShouldUpdateFieldsAndTimestamp()
    {
        // Arrange
        var employee = CreateValidEmployee();

        // Act
        employee.UpdateContactInfo("New Name", "111222", "333444");

        // Assert
        employee.Name.Should().Be("New Name");
        employee.Telephone.Should().Be("111222");
        employee.Fax.Should().Be("333444");
        employee.UpdatedOn.Should().NotBeNull();
    }

    [Fact]
    public void UpdateCompanyId_WithValidId_ShouldSucceed()
    {
        // Arrange
        var employee = CreateValidEmployee();

        // Act
        employee.UpdateCompanyId(5);

        // Assert
        employee.CompanyId.Should().Be(5);
        employee.UpdatedOn.Should().NotBeNull();
    }

    [Fact]
    public void UpdateCompanyId_WithInvalidId_ShouldThrowDomainException()
    {
        // Arrange
        var employee = CreateValidEmployee();

        // Act
        var act = () => employee.UpdateCompanyId(0);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("CompanyId debe ser mayor que cero.");
    }

    [Fact]
    public void RecordLogin_ShouldUpdateLastLoginAndTimestamp()
    {
        // Arrange
        var employee = CreateValidEmployee();

        // Act
        employee.RecordLogin();

        // Assert
        employee.LastLogin.Should().NotBeNull();
        employee.LastLogin.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        employee.UpdatedOn.Should().NotBeNull();
    }

    [Fact]
    public void VerifyPassword_WithCorrectPassword_ShouldReturnTrue()
    {
        // Arrange
        var employee = CreateValidEmployee();

        // Act
        var result = employee.VerifyPassword(ValidPassword);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void VerifyPassword_WithIncorrectPassword_ShouldReturnFalse()
    {
        // Arrange
        var employee = CreateValidEmployee();

        // Act
        var result = employee.VerifyPassword("WrongPassword");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Delete_ShouldSetDeletedOnTimestamp()
    {
        // Arrange
        var employee = CreateValidEmployee();

        // Act
        employee.Delete();

        // Assert
        employee.IsDeleted.Should().BeTrue();
        employee.DeletedOn.Should().NotBeNull();
        employee.DeletedOn.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Delete_WhenAlreadyDeleted_ShouldThrowDomainException()
    {
        // Arrange
        var employee = CreateValidEmployee();
        employee.Delete();

        // Act
        var act = () => employee.Delete();

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("El empleado ya fue eliminado.");
    }

    [Fact]
    public void Reconstitute_ShouldCreateEmployeeWithAllFields()
    {
        // Arrange
        var id = 1;
        var createdOn = DateTime.UtcNow.AddDays(-10);
        var updatedOn = DateTime.UtcNow.AddDays(-5);
        var lastLogin = DateTime.UtcNow.AddDays(-1);
        var passwordHash = "hashedpassword";

        // Act
        var employee = Employee.Reconstitute(
            id: id,
            companyId: ValidCompanyId,
            createdOn: createdOn,
            deletedOn: null,
            email: ValidEmail,
            fax: "123456",
            name: "John Doe",
            lastLogin: lastLogin,
            passwordHash: passwordHash,
            portalId: ValidPortalId,
            roleId: ValidRoleId,
            statusId: ValidStatusId,
            telephone: "789012",
            updatedOn: updatedOn,
            username: ValidUsername);

        // Assert
        employee.Id.Should().Be(id);
        employee.CompanyId.Should().Be(ValidCompanyId);
        employee.CreatedOn.Should().Be(createdOn);
        employee.UpdatedOn.Should().Be(updatedOn);
        employee.LastLogin.Should().Be(lastLogin);
        employee.IsDeleted.Should().BeFalse();
    }

    private static Employee CreateValidEmployee()
    {
        return Employee.Create(
            companyId: ValidCompanyId,
            email: ValidEmail,
            password: ValidPassword,
            portalId: ValidPortalId,
            roleId: ValidRoleId,
            statusId: ValidStatusId,
            username: ValidUsername);
    }
}
