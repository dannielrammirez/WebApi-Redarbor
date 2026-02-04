using FluentAssertions;
using Redarbor.Application.DTOs;
using Redarbor.Application.Employees.Commands.CreateEmployee;
using Xunit;

namespace Redarbor.API.Tests.Validators;

public class CreateEmployeeCommandValidatorTests
{
    private readonly CreateEmployeeCommandValidator _validator;

    public CreateEmployeeCommandValidatorTests()
    {
        _validator = new CreateEmployeeCommandValidator();
    }

    [Fact]
    public async Task Validate_WithValidData_ShouldBeValid()
    {
        // Arrange
        var command = new CreateEmployeeCommand
        {
            Employee = new CreateEmployeeDto
            {
                CompanyId = 1,
                Email = "test@example.com",
                Password = "SecurePassword123",
                PortalId = 1,
                RoleId = 1,
                StatusId = 1,
                Username = "testuser",
                Name = "Test User"
            }
        };

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validate_WithInvalidCompanyId_ShouldBeInvalid(int companyId)
    {
        // Arrange
        var command = new CreateEmployeeCommand
        {
            Employee = new CreateEmployeeDto
            {
                CompanyId = companyId,
                Email = "test@example.com",
                Password = "SecurePassword123",
                PortalId = 1,
                RoleId = 1,
                StatusId = 1,
                Username = "testuser"
            }
        };

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName.Contains("CompanyId"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("invalid")]
    [InlineData("invalid@")]
    public async Task Validate_WithInvalidEmail_ShouldBeInvalid(string email)
    {
        // Arrange
        var command = new CreateEmployeeCommand
        {
            Employee = new CreateEmployeeDto
            {
                CompanyId = 1,
                Email = email,
                Password = "SecurePassword123",
                PortalId = 1,
                RoleId = 1,
                StatusId = 1,
                Username = "testuser"
            }
        };

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName.Contains("Email"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("12345")]
    public async Task Validate_WithInvalidPassword_ShouldBeInvalid(string password)
    {
        // Arrange
        var command = new CreateEmployeeCommand
        {
            Employee = new CreateEmployeeDto
            {
                CompanyId = 1,
                Email = "test@example.com",
                Password = password,
                PortalId = 1,
                RoleId = 1,
                StatusId = 1,
                Username = "testuser"
            }
        };

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName.Contains("Password"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("ab")]
    [InlineData("test user")]
    [InlineData("test@user")]
    public async Task Validate_WithInvalidUsername_ShouldBeInvalid(string username)
    {
        // Arrange
        var command = new CreateEmployeeCommand
        {
            Employee = new CreateEmployeeDto
            {
                CompanyId = 1,
                Email = "test@example.com",
                Password = "SecurePassword123",
                PortalId = 1,
                RoleId = 1,
                StatusId = 1,
                Username = username
            }
        };

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName.Contains("Username"));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validate_WithInvalidPortalId_ShouldBeInvalid(int portalId)
    {
        // Arrange
        var command = new CreateEmployeeCommand
        {
            Employee = new CreateEmployeeDto
            {
                CompanyId = 1,
                Email = "test@example.com",
                Password = "SecurePassword123",
                PortalId = portalId,
                RoleId = 1,
                StatusId = 1,
                Username = "testuser"
            }
        };

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName.Contains("PortalId"));
    }
}
