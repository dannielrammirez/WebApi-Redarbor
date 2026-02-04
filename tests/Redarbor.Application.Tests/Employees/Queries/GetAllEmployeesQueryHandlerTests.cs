using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Redarbor.Application.Employees.Queries.GetAllEmployees;
using Redarbor.Domain.Interfaces;
using Xunit;

namespace Redarbor.Application.Tests.Employees.Queries;

public class GetAllEmployeesQueryHandlerTests
{
    private readonly Mock<IEmployeeReadRepository> _readRepositoryMock;
    private readonly Mock<ILogger<GetAllEmployeesQueryHandler>> _loggerMock;
    private readonly GetAllEmployeesQueryHandler _handler;

    public GetAllEmployeesQueryHandlerTests()
    {
        _readRepositoryMock = new Mock<IEmployeeReadRepository>();
        _loggerMock = new Mock<ILogger<GetAllEmployeesQueryHandler>>();

        _handler = new GetAllEmployeesQueryHandler(_readRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnAllEmployees()
    {
        // Arrange
        var employees = new List<EmployeeReadModel>
        {
            new()
            {
                Id = 1,
                CompanyId = 1,
                Email = "test1@example.com",
                Username = "user1",
                CreatedOn = DateTime.UtcNow,
                PortalId = 1,
                RoleId = 1,
                StatusId = 1
            },
            new()
            {
                Id = 2,
                CompanyId = 1,
                Email = "test2@example.com",
                Username = "user2",
                CreatedOn = DateTime.UtcNow,
                PortalId = 1,
                RoleId = 1,
                StatusId = 1
            }
        };

        _readRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(employees);

        var query = new GetAllEmployeesQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_WithNoEmployees_ShouldReturnEmptyList()
    {
        // Arrange
        _readRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Enumerable.Empty<EmployeeReadModel>());

        var query = new GetAllEmployeesQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }
}
