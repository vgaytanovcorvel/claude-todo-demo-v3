using Corvel.ToDo.Abstractions.Exceptions;
using Corvel.ToDo.Abstractions.Models;
using Corvel.ToDo.Repository.Contexts;
using Corvel.ToDo.Repository.Repositories;
using Corvel.ToDo.Repository.Tests.Helpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Corvel.ToDo.Repository.Tests.Repositories;

[TestClass]
public class UserRepositoryTests
{
    private DbContextOptions<ToDoDbContext> dbContextOptions = null!;
    private IDbContextFactory<ToDoDbContext> contextFactory = null!;
    private UserRepository repository = null!;

    [TestInitialize]
    public void Setup()
    {
        dbContextOptions = new DbContextOptionsBuilder<ToDoDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        contextFactory = new TestDbContextFactory(dbContextOptions);
        repository = new UserRepository(contextFactory);
    }

    [TestMethod]
    public async Task UserAddAsync_ShouldReturnCreatedUser_WhenUserIsValid()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var user = new User
        {
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            PasswordHash = "hashed_password_123",
            CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        // Act
        var result = await repository.UserAddAsync(user, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.Email.Should().Be("test@example.com");
        result.FirstName.Should().Be("John");
        result.LastName.Should().Be("Doe");
        result.PasswordHash.Should().Be("hashed_password_123");
        result.CreatedAtUtc.Should().Be(new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc));
    }

    [TestMethod]
    public async Task UserSingleByIdAsync_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var user = new User
        {
            Email = "existing@example.com",
            FirstName = "Jane",
            LastName = "Smith",
            PasswordHash = "hashed_pw",
            CreatedAtUtc = new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        var added = await repository.UserAddAsync(user, cancellationToken);

        // Act
        var result = await repository.UserSingleByIdAsync(added.Id, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(added.Id);
        result.Email.Should().Be("existing@example.com");
        result.FirstName.Should().Be("Jane");
        result.LastName.Should().Be("Smith");
    }

    [TestMethod]
    public async Task UserSingleByIdAsync_ShouldThrowNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var nonExistentId = 999;

        // Act & Assert
        var exception = await Assert.ThrowsExceptionAsync<NotFoundException>(
            () => repository.UserSingleByIdAsync(nonExistentId, cancellationToken));

        exception.Message.Should().Contain("User not found");
        exception.Message.Should().Contain("999");
    }

    [TestMethod]
    public async Task UserSingleOrDefaultByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var nonExistentId = 999;

        // Act
        var result = await repository.UserSingleOrDefaultByIdAsync(nonExistentId, cancellationToken);

        // Assert
        result.Should().BeNull();
    }

    [TestMethod]
    public async Task UserSingleOrDefaultByEmailAsync_ShouldReturnUser_WhenEmailExists()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var user = new User
        {
            Email = "findme@example.com",
            FirstName = "Find",
            LastName = "Me",
            PasswordHash = "hashed_pw",
            CreatedAtUtc = new DateTime(2026, 3, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        await repository.UserAddAsync(user, cancellationToken);

        // Act
        var result = await repository.UserSingleOrDefaultByEmailAsync("findme@example.com", cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be("findme@example.com");
        result.FirstName.Should().Be("Find");
        result.LastName.Should().Be("Me");
    }

    [TestMethod]
    public async Task UserSingleOrDefaultByEmailAsync_ShouldReturnNull_WhenEmailDoesNotExist()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await repository.UserSingleOrDefaultByEmailAsync("nonexistent@example.com", cancellationToken);

        // Assert
        result.Should().BeNull();
    }

    [TestMethod]
    public async Task UserUpdateAsync_ShouldReturnUpdatedUser_WhenUserExists()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var user = new User
        {
            Email = "original@example.com",
            FirstName = "Original",
            LastName = "Name",
            PasswordHash = "original_hash",
            CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        var added = await repository.UserAddAsync(user, cancellationToken);

        var updatedUser = new User
        {
            Id = added.Id,
            Email = "updated@example.com",
            FirstName = "Updated",
            LastName = "User",
            PasswordHash = "new_hash",
            CreatedAtUtc = added.CreatedAtUtc,
            UpdatedAtUtc = new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        // Act
        var result = await repository.UserUpdateAsync(updatedUser, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(added.Id);
        result.Email.Should().Be("updated@example.com");
        result.FirstName.Should().Be("Updated");
        result.LastName.Should().Be("User");
        result.PasswordHash.Should().Be("new_hash");
        result.UpdatedAtUtc.Should().Be(new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc));
    }

    [TestMethod]
    public async Task UserUpdateAsync_ShouldThrowNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var nonExistentUser = new User
        {
            Id = 999,
            Email = "ghost@example.com",
            FirstName = "Ghost",
            LastName = "User",
            PasswordHash = "hash",
            CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        // Act & Assert
        var exception = await Assert.ThrowsExceptionAsync<NotFoundException>(
            () => repository.UserUpdateAsync(nonExistentUser, cancellationToken));

        exception.Message.Should().Contain("User not found");
        exception.Message.Should().Contain("999");
    }

    [TestCleanup]
    public void Cleanup()
    {
        using var context = new ToDoDbContext(dbContextOptions);
        context.Database.EnsureDeleted();
    }

}
