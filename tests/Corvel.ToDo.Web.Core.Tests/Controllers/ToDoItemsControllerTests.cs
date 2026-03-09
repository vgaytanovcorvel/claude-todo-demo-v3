using System.Net;
using Corvel.ToDo.Abstractions.Interfaces;
using Corvel.ToDo.Abstractions.Models;
using Corvel.ToDo.Abstractions.Requests;
using Corvel.ToDo.Common.Dtos;
using Corvel.ToDo.Common.Enums;
using Corvel.ToDo.Web.Core.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Corvel.ToDo.Web.Core.Tests.Controllers;

[TestClass]
public class ToDoItemsControllerTests
{
    private Mock<IToDoItemService> toDoItemServiceMock = new(MockBehavior.Strict);
    private Mock<ToDoItemsController> controllerMock = null!;
    private CancellationToken cancellationToken = CancellationToken.None;

    [TestInitialize]
    public void Setup()
    {
        controllerMock = new Mock<ToDoItemsController>(
            () => new ToDoItemsController(toDoItemServiceMock.Object),
            MockBehavior.Strict);
    }

    [TestMethod]
    public void Class_ShouldHaveAuthorizeAttribute()
    {
        // Arrange & Act
        var authorizeAttribute = typeof(ToDoItemsController)
            .GetCustomAttributes(typeof(AuthorizeAttribute), inherit: true);

        // Assert
        authorizeAttribute.Should().HaveCount(1);
    }

    [TestMethod]
    public async Task GetAll_ShouldReturnOkWithItems_WhenItemsExist()
    {
        // Arrange
        var items = new List<ToDoItem>
        {
            new()
            {
                Id = 1,
                Title = "Test Item",
                Priority = Priority.Medium,
                Status = ToDoItemStatus.Pending,
                CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        };

        toDoItemServiceMock
            .Setup(service => service.GetAllAsync(cancellationToken))
            .ReturnsAsync(items)
            .Verifiable(Times.Once());

        controllerMock
            .Setup(controller => controller.GetAll(cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        controllerMock
            .Setup(controller => controller.Ok(It.IsAny<object>()))
            .CallBase();

        // Act
        var result = await controllerMock.Object.GetAll(cancellationToken);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        var apiResponse = okResult.Value as ApiResponse<IReadOnlyList<ToDoItem>>;
        apiResponse.Should().NotBeNull();
        apiResponse!.Success.Should().BeTrue();
        apiResponse.Data.Should().HaveCount(1);
        apiResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        controllerMock.VerifyAll();
        toDoItemServiceMock.VerifyAll();
    }

    [TestMethod]
    public async Task GetAll_ShouldReturnOkWithEmptyList_WhenNoItemsExist()
    {
        // Arrange
        var items = new List<ToDoItem>();

        toDoItemServiceMock
            .Setup(service => service.GetAllAsync(cancellationToken))
            .ReturnsAsync(items)
            .Verifiable(Times.Once());

        controllerMock
            .Setup(controller => controller.GetAll(cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        controllerMock
            .Setup(controller => controller.Ok(It.IsAny<object>()))
            .CallBase();

        // Act
        var result = await controllerMock.Object.GetAll(cancellationToken);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        var apiResponse = okResult.Value as ApiResponse<IReadOnlyList<ToDoItem>>;
        apiResponse.Should().NotBeNull();
        apiResponse!.Success.Should().BeTrue();
        apiResponse.Data.Should().BeEmpty();
        apiResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        controllerMock.VerifyAll();
        toDoItemServiceMock.VerifyAll();
    }

    [TestMethod]
    public async Task GetById_ShouldReturnOkWithItem_WhenItemExists()
    {
        // Arrange
        var itemId = 1;
        var item = new ToDoItem
        {
            Id = itemId,
            Title = "Test Item",
            Priority = Priority.High,
            Status = ToDoItemStatus.Pending,
            CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        toDoItemServiceMock
            .Setup(service => service.GetByIdAsync(itemId, cancellationToken))
            .ReturnsAsync(item)
            .Verifiable(Times.Once());

        controllerMock
            .Setup(controller => controller.GetById(itemId, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        controllerMock
            .Setup(controller => controller.Ok(It.IsAny<object>()))
            .CallBase();

        // Act
        var result = await controllerMock.Object.GetById(itemId, cancellationToken);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        var apiResponse = okResult.Value as ApiResponse<ToDoItem>;
        apiResponse.Should().NotBeNull();
        apiResponse!.Success.Should().BeTrue();
        apiResponse.Data!.Id.Should().Be(itemId);
        apiResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        controllerMock.VerifyAll();
        toDoItemServiceMock.VerifyAll();
    }

    [TestMethod]
    public async Task GetById_ShouldReturnNotFound_WhenItemDoesNotExist()
    {
        // Arrange
        var itemId = 999;

        toDoItemServiceMock
            .Setup(service => service.GetByIdAsync(itemId, cancellationToken))
            .ReturnsAsync((ToDoItem?)null)
            .Verifiable(Times.Once());

        controllerMock
            .Setup(controller => controller.GetById(itemId, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        controllerMock
            .Setup(controller => controller.NotFound(It.IsAny<object>()))
            .CallBase();

        // Act
        var result = await controllerMock.Object.GetById(itemId, cancellationToken);

        // Assert
        var notFoundResult = result.Result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);

        var apiResponse = notFoundResult.Value as ApiResponse<ToDoItem>;
        apiResponse.Should().NotBeNull();
        apiResponse!.Success.Should().BeFalse();
        apiResponse.Error.Should().Contain("999");
        apiResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);

        controllerMock.VerifyAll();
        toDoItemServiceMock.VerifyAll();
    }

    [TestMethod]
    public async Task Create_ShouldReturnCreatedWithItem_WhenRequestIsValid()
    {
        // Arrange
        var request = new CreateToDoItemRequest(
            "New Item",
            "Description",
            Priority.High,
            new DateTime(2026, 12, 31, 0, 0, 0, DateTimeKind.Utc));

        var createdItem = new ToDoItem
        {
            Id = 1,
            Title = "New Item",
            Description = "Description",
            Priority = Priority.High,
            Status = ToDoItemStatus.Pending,
            CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            DueDate = new DateTime(2026, 12, 31, 0, 0, 0, DateTimeKind.Utc)
        };

        toDoItemServiceMock
            .Setup(service => service.CreateAsync(request, cancellationToken))
            .ReturnsAsync(createdItem)
            .Verifiable(Times.Once());

        controllerMock
            .Setup(controller => controller.Create(request, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        controllerMock
            .Setup(controller => controller.CreatedAtAction(
                It.IsAny<string>(), It.IsAny<object>(), It.IsAny<object>()))
            .CallBase();

        controllerMock
            .Setup(controller => controller.CreatedAtAction(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<object>()))
            .CallBase();

        // Act
        var result = await controllerMock.Object.Create(request, cancellationToken);

        // Assert
        var createdResult = result.Result as CreatedAtActionResult;
        createdResult.Should().NotBeNull();
        createdResult!.StatusCode.Should().Be(201);

        var apiResponse = createdResult.Value as ApiResponse<ToDoItem>;
        apiResponse.Should().NotBeNull();
        apiResponse!.Success.Should().BeTrue();
        apiResponse.Data!.Id.Should().Be(1);
        apiResponse.Data.Title.Should().Be("New Item");
        apiResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        controllerMock.VerifyAll();
        toDoItemServiceMock.VerifyAll();
    }

    [TestMethod]
    public async Task Update_ShouldReturnOkWithItem_WhenRequestIsValid()
    {
        // Arrange
        var itemId = 1;
        var request = new UpdateToDoItemRequest(
            "Updated Item",
            "Updated Description",
            Priority.Low,
            ToDoItemStatus.InProgress,
            new DateTime(2026, 12, 31, 0, 0, 0, DateTimeKind.Utc));

        var updatedItem = new ToDoItem
        {
            Id = itemId,
            Title = "Updated Item",
            Description = "Updated Description",
            Priority = Priority.Low,
            Status = ToDoItemStatus.InProgress,
            CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            UpdatedAtUtc = new DateTime(2026, 3, 9, 0, 0, 0, DateTimeKind.Utc)
        };

        toDoItemServiceMock
            .Setup(service => service.UpdateAsync(itemId, request, cancellationToken))
            .ReturnsAsync(updatedItem)
            .Verifiable(Times.Once());

        controllerMock
            .Setup(controller => controller.Update(itemId, request, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        controllerMock
            .Setup(controller => controller.Ok(It.IsAny<object>()))
            .CallBase();

        // Act
        var result = await controllerMock.Object.Update(itemId, request, cancellationToken);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        var apiResponse = okResult.Value as ApiResponse<ToDoItem>;
        apiResponse.Should().NotBeNull();
        apiResponse!.Success.Should().BeTrue();
        apiResponse.Data!.Title.Should().Be("Updated Item");
        apiResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        controllerMock.VerifyAll();
        toDoItemServiceMock.VerifyAll();
    }

    [TestMethod]
    public async Task Delete_ShouldReturnNoContent_WhenItemExists()
    {
        // Arrange
        var itemId = 1;

        toDoItemServiceMock
            .Setup(service => service.DeleteAsync(itemId, cancellationToken))
            .Returns(Task.CompletedTask)
            .Verifiable(Times.Once());

        controllerMock
            .Setup(controller => controller.Delete(itemId, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        controllerMock
            .Setup(controller => controller.NoContent())
            .CallBase();

        // Act
        var result = await controllerMock.Object.Delete(itemId, cancellationToken);

        // Assert
        var noContentResult = result as NoContentResult;
        noContentResult.Should().NotBeNull();
        noContentResult!.StatusCode.Should().Be(204);

        controllerMock.VerifyAll();
        toDoItemServiceMock.VerifyAll();
    }
}
