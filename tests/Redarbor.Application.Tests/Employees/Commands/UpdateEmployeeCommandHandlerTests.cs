using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Redarbor.Application.DTOs;
using Redarbor.Application.Employees.Commands.UpdateEmployee;
using Redarbor.Domain.Entities;
using Redarbor.Domain.Interfaces;
using Xunit;

namespace Redarbor.Application.Tests.Employees.Commands;

public class UpdateEmployeeCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IEmployeeRepository> _employeeRepositoryMock;
    private readonly Mock<ILogger<UpdateEmployeeCommandHandler>> _loggerMock;
    private readonly UpdateEmployeeCommandHandler _handler;

    public UpdateEmployeeCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _employeeRepositoryMock = new Mock<IEmployeeRepository>();
        _loggerMock = new Mock<ILogger<UpdateEmployeeCommandHandler>>();

        _unitOfWorkMock.Setup(x => x.Employees).Returns(_employeeRepositoryMock.Object);

        _handler = new UpdateEmployeeCommandHandler(_unitOfWorkMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidData_ShouldUpdateEmployee()
    {
        // Arrange
        var existingEmployee = Employee.Create(
            companyId: 1,
            email: "test@example.com",
            password: "SecurePassword123",
            portalId: 1,
            roleId: 1,
            statusId: 1,
            username: "testuser");

        var dto = new UpdateEmployeeDto
        {
            Username = "updateduser",
            Name = "Updated Name"
        };

        var command = new UpdateEmployeeCommand { Id = 1, Employee = dto };

        _employeeRepositoryMock
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingEmployee);

        _employeeRepositoryMock
            .Setup(x => x.UsernameExistsAsync(dto.Username, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _employeeRepositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistingEmployee_ShouldReturnFailure()
    {
        // Arrange
        var dto = new UpdateEmployeeDto { Username = "newuser" };
        var command = new UpdateEmployeeCommand { Id = 999, Employee = dto };

        _employeeRepositoryMock
            .Setup(x => x.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Employee?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("no fue encontrado");

        _employeeRepositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WithDeletedEmployee_ShouldReturnFailure()
    {
        // Arrange
        var deletedEmployee = Employee.Create(
            companyId: 1,
            email: "test@example.com",
            password: "SecurePassword123",
            portalId: 1,
            roleId: 1,
            statusId: 1,
            username: "testuser");
        deletedEmployee.Delete();

        var dto = new UpdateEmployeeDto { Username = "newuser" };
        var command = new UpdateEmployeeCommand { Id = 1, Employee = dto };

        _employeeRepositoryMock
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(deletedEmployee);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("deleted");
    }

    [Fact]
    public async Task Handle_WithDuplicateUsername_ShouldReturnFailure()
    {
        // Arrange
        var existingEmployee = Employee.Create(
            companyId: 1,
            email: "test@example.com",
            password: "SecurePassword123",
            portalId: 1,
            roleId: 1,
            statusId: 1,
            username: "testuser");

        var dto = new UpdateEmployeeDto { Username = "existinguser" };
        var command = new UpdateEmployeeCommand { Id = 1, Employee = dto };

        _employeeRepositoryMock
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingEmployee);

        _employeeRepositoryMock
            .Setup(x => x.UsernameExistsAsync("existinguser", 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("ya existe");
    }
}
