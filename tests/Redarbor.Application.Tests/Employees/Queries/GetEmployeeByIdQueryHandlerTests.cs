using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Redarbor.Application.Employees.Queries.GetEmployeeById;
using Redarbor.Domain.Interfaces;
using Xunit;

namespace Redarbor.Application.Tests.Employees.Queries;

public class GetEmployeeByIdQueryHandlerTests
{
    private readonly Mock<IEmployeeReadRepository> _readRepositoryMock;
    private readonly Mock<ILogger<GetEmployeeByIdQueryHandler>> _loggerMock;
    private readonly GetEmployeeByIdQueryHandler _handler;

    public GetEmployeeByIdQueryHandlerTests()
    {
        _readRepositoryMock = new Mock<IEmployeeReadRepository>();
        _loggerMock = new Mock<ILogger<GetEmployeeByIdQueryHandler>>();

        _handler = new GetEmployeeByIdQueryHandler(_readRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WithExistingEmployee_ShouldReturnEmployee()
    {
        // Arrange
        var employee = new EmployeeReadModel
        {
            Id = 1,
            CompanyId = 1,
            Email = "test@example.com",
            Username = "testuser",
            Name = "Test User",
            CreatedOn = DateTime.UtcNow,
            PortalId = 1,
            RoleId = 1,
            StatusId = 1
        };

        _readRepositoryMock
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employee);

        var query = new GetEmployeeByIdQuery { Id = 1 };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Id.Should().Be(1);
        result.Value.Username.Should().Be("testuser");
    }

    [Fact]
    public async Task Handle_WithNonExistingEmployee_ShouldReturnFailure()
    {
        // Arrange
        _readRepositoryMock
            .Setup(x => x.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((EmployeeReadModel?)null);

        var query = new GetEmployeeByIdQuery { Id = 999 };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("no fue encontrado");
    }
}
