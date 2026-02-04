using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Redarbor.Application.DTOs;
using Redarbor.Application.Employees.Commands.CreateEmployee;
using Redarbor.Domain.Entities;
using Redarbor.Domain.Interfaces;
using Xunit;

namespace Redarbor.Application.Tests.Employees.Commands;

public class CreateEmployeeCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IEmployeeRepository> _employeeRepositoryMock;
    private readonly Mock<ILogger<CreateEmployeeCommandHandler>> _loggerMock;
    private readonly CreateEmployeeCommandHandler _handler;

    public CreateEmployeeCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _employeeRepositoryMock = new Mock<IEmployeeRepository>();
        _loggerMock = new Mock<ILogger<CreateEmployeeCommandHandler>>();

        _unitOfWorkMock.Setup(x => x.Employees).Returns(_employeeRepositoryMock.Object);

        _handler = new CreateEmployeeCommandHandler(_unitOfWorkMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidData_ShouldCreateEmployee()
    {
        // Arrange
        var dto = new CreateEmployeeDto
        {
            CompanyId = 1,
            Email = "test@example.com",
            Password = "SecurePassword123",
            PortalId = 1,
            RoleId = 1,
            StatusId = 1,
            Username = "testuser",
            Name = "Test User"
        };

        var command = new CreateEmployeeCommand { Employee = dto };

        _employeeRepositoryMock
            .Setup(x => x.UsernameExistsAsync(dto.Username, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _employeeRepositoryMock
            .Setup(x => x.EmailExistsAsync(dto.Email, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _employeeRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Employee e, CancellationToken _) => e);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        //result.Value.Should().BeGreaterThanOrEqualTo(0);

        _employeeRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithDuplicateUsername_ShouldReturnFailure()
    {
        // Arrange
        var dto = new CreateEmployeeDto
        {
            CompanyId = 1,
            Email = "test@example.com",
            Password = "SecurePassword123",
            PortalId = 1,
            RoleId = 1,
            StatusId = 1,
            Username = "existinguser",
            Name = "Test User"
        };

        var command = new CreateEmployeeCommand { Employee = dto };

        _employeeRepositoryMock
            .Setup(x => x.UsernameExistsAsync(dto.Username, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("ya existe");

        _employeeRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WithDuplicateEmail_ShouldReturnFailure()
    {
        // Arrange
        var dto = new CreateEmployeeDto
        {
            CompanyId = 1,
            Email = "existing@example.com",
            Password = "SecurePassword123",
            PortalId = 1,
            RoleId = 1,
            StatusId = 1,
            Username = "testuser",
            Name = "Test User"
        };

        var command = new CreateEmployeeCommand { Employee = dto };

        _employeeRepositoryMock
            .Setup(x => x.UsernameExistsAsync(dto.Username, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _employeeRepositoryMock
            .Setup(x => x.EmailExistsAsync(dto.Email, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("ya existe");

        _employeeRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WithInvalidEmail_ShouldReturnFailure()
    {
        // Arrange
        var dto = new CreateEmployeeDto
        {
            CompanyId = 1,
            Email = "invalid-email",
            Password = "SecurePassword123",
            PortalId = 1,
            RoleId = 1,
            StatusId = 1,
            Username = "testuser",
            Name = "Test User"
        };

        var command = new CreateEmployeeCommand { Employee = dto };

        _employeeRepositoryMock
            .Setup(x => x.UsernameExistsAsync(dto.Username, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _employeeRepositoryMock
            .Setup(x => x.EmailExistsAsync(dto.Email, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("email");
    }
}
