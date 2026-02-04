using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Redarbor.Application.Employees.Commands.DeleteEmployee;
using Redarbor.Domain.Entities;
using Redarbor.Domain.Interfaces;
using Xunit;

namespace Redarbor.Application.Tests.Employees.Commands;

public class DeleteEmployeeCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IEmployeeRepository> _employeeRepositoryMock;
    private readonly Mock<ILogger<DeleteEmployeeCommandHandler>> _loggerMock;
    private readonly DeleteEmployeeCommandHandler _handler;

    public DeleteEmployeeCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _employeeRepositoryMock = new Mock<IEmployeeRepository>();
        _loggerMock = new Mock<ILogger<DeleteEmployeeCommandHandler>>();

        _unitOfWorkMock.Setup(x => x.Employees).Returns(_employeeRepositoryMock.Object);

        _handler = new DeleteEmployeeCommandHandler(_unitOfWorkMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WithExistingEmployee_ShouldDeleteEmployee()
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

        var command = new DeleteEmployeeCommand { Id = 1 };

        _employeeRepositoryMock
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingEmployee);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        existingEmployee.IsDeleted.Should().BeTrue();

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
        var command = new DeleteEmployeeCommand { Id = 999 };

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
    public async Task Handle_WithAlreadyDeletedEmployee_ShouldReturnFailure()
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

        var command = new DeleteEmployeeCommand { Id = 1 };

        _employeeRepositoryMock
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(deletedEmployee);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("already deleted");
    }
}
