using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Corvel.ToDo.Abstractions.Models;
using Corvel.ToDo.Abstractions.Requests;
using Corvel.ToDo.Common.Dtos;
using Corvel.ToDo.Common.Enums;
using Corvel.ToDo.Web.Server.Tests.Infrastructure;
using FluentAssertions;

namespace Corvel.ToDo.Web.Server.Tests.Integration;

[TestClass]
public class ToDoItemsIntegrationTests
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private const string BaseUrl = "/api/todo-items";

    private ToDoWebApplicationFactory factory = null!;
    private HttpClient client = null!;

    [TestInitialize]
    public void Setup()
    {
        factory = new ToDoWebApplicationFactory();
        client = factory.CreateClient();
    }

    [TestCleanup]
    public void Cleanup()
    {
        client.Dispose();
        factory.Dispose();
    }

    [TestMethod]
    public async Task GetAll_ShouldReturn200WithEmptyList_WhenNoItemsExist()
    {
        // Arrange
        // No items created

        // Act
        var response = await client.GetAsync(BaseUrl);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var apiResponse = await DeserializeResponseAsync<IReadOnlyList<ToDoItem>>(response);

        apiResponse.Should().NotBeNull();
        var result = apiResponse ?? throw new InvalidOperationException("Response was null");
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Should().BeEmpty();
    }

    [TestMethod]
    public async Task GetAll_ShouldReturn200WithItems_WhenItemsExist()
    {
        // Arrange
        var request1 = new CreateToDoItemRequest(
            "First Item",
            "First Description",
            Priority.Medium,
            null);

        var request2 = new CreateToDoItemRequest(
            "Second Item",
            "Second Description",
            Priority.High,
            null);

        await client.PostAsJsonAsync(BaseUrl, request1);
        await client.PostAsJsonAsync(BaseUrl, request2);

        // Act
        var response = await client.GetAsync(BaseUrl);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var apiResponse = await DeserializeResponseAsync<IReadOnlyList<ToDoItem>>(response);

        apiResponse.Should().NotBeNull();
        var result = apiResponse ?? throw new InvalidOperationException("Response was null");
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        var data = result.Data ?? throw new InvalidOperationException("Data was null");
        data.Should().HaveCount(2);
        data.Should().Contain(item => item.Title == "First Item");
        data.Should().Contain(item => item.Title == "Second Item");
    }

    [TestMethod]
    public async Task Create_ShouldReturn201WithCreatedItem_WhenRequestIsValid()
    {
        // Arrange
        var request = new CreateToDoItemRequest(
            "Test Item",
            "Test Description",
            Priority.Medium,
            null);

        // Act
        var response = await client.PostAsJsonAsync(BaseUrl, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var apiResponse = await DeserializeResponseAsync<ToDoItem>(response);

        apiResponse.Should().NotBeNull();
        var result = apiResponse ?? throw new InvalidOperationException("Response was null");
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        var data = result.Data ?? throw new InvalidOperationException("Data was null");
        data.Title.Should().Be("Test Item");
        data.Description.Should().Be("Test Description");
        data.Priority.Should().Be(Priority.Medium);
        data.Status.Should().Be(ToDoItemStatus.Pending);
        data.Id.Should().BeGreaterThan(0);
    }

    [TestMethod]
    public async Task Create_ShouldReturn400_WhenTitleIsEmpty()
    {
        // Arrange
        var request = new CreateToDoItemRequest(
            "",
            "Test Description",
            Priority.Medium,
            null);

        // Act
        var response = await client.PostAsJsonAsync(BaseUrl, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrWhiteSpace();
    }

    [TestMethod]
    public async Task GetById_ShouldReturn200WithItem_WhenItemExists()
    {
        // Arrange
        var createRequest = new CreateToDoItemRequest(
            "Get By Id Item",
            "Description",
            Priority.High,
            null);

        var createResponse = await client.PostAsJsonAsync(BaseUrl, createRequest);
        var createdApiResponse = await DeserializeResponseAsync<ToDoItem>(createResponse);
        createdApiResponse.Should().NotBeNull();
        var createdResult = createdApiResponse ?? throw new InvalidOperationException("Response was null");
        createdResult.Data.Should().NotBeNull();
        var createdData = createdResult.Data ?? throw new InvalidOperationException("Data was null");
        var createdItemId = createdData.Id;

        // Act
        var response = await client.GetAsync($"{BaseUrl}/{createdItemId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var apiResponse = await DeserializeResponseAsync<ToDoItem>(response);

        apiResponse.Should().NotBeNull();
        var result = apiResponse ?? throw new InvalidOperationException("Response was null");
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        var data = result.Data ?? throw new InvalidOperationException("Data was null");
        data.Id.Should().Be(createdItemId);
        data.Title.Should().Be("Get By Id Item");
        data.Description.Should().Be("Description");
        data.Priority.Should().Be(Priority.High);
    }

    [TestMethod]
    public async Task GetById_ShouldReturn404_WhenItemDoesNotExist()
    {
        // Arrange
        var nonExistentId = 99999;

        // Act
        var response = await client.GetAsync($"{BaseUrl}/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var apiResponse = await DeserializeResponseAsync<ToDoItem>(response);

        apiResponse.Should().NotBeNull();
        var result = apiResponse ?? throw new InvalidOperationException("Response was null");
        result.Success.Should().BeFalse();
        result.Error.Should().NotBeNullOrWhiteSpace();
    }

    [TestMethod]
    public async Task Update_ShouldReturn200WithUpdatedItem_WhenRequestIsValid()
    {
        // Arrange
        var createRequest = new CreateToDoItemRequest(
            "Original Title",
            "Original Description",
            Priority.Low,
            null);

        var createResponse = await client.PostAsJsonAsync(BaseUrl, createRequest);
        var createdApiResponse = await DeserializeResponseAsync<ToDoItem>(createResponse);
        createdApiResponse.Should().NotBeNull();
        var createdResult = createdApiResponse ?? throw new InvalidOperationException("Response was null");
        createdResult.Data.Should().NotBeNull();
        var createdData = createdResult.Data ?? throw new InvalidOperationException("Data was null");
        var createdItemId = createdData.Id;

        var updateRequest = new UpdateToDoItemRequest(
            "Updated Title",
            "Updated Description",
            Priority.Critical,
            ToDoItemStatus.InProgress,
            null);

        // Act
        var response = await client.PutAsJsonAsync($"{BaseUrl}/{createdItemId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var apiResponse = await DeserializeResponseAsync<ToDoItem>(response);

        apiResponse.Should().NotBeNull();
        var result = apiResponse ?? throw new InvalidOperationException("Response was null");
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        var data = result.Data ?? throw new InvalidOperationException("Data was null");
        data.Id.Should().Be(createdItemId);
        data.Title.Should().Be("Updated Title");
        data.Description.Should().Be("Updated Description");
        data.Priority.Should().Be(Priority.Critical);
        data.Status.Should().Be(ToDoItemStatus.InProgress);
    }

    [TestMethod]
    public async Task Update_ShouldReturn400_WhenTitleIsEmpty()
    {
        // Arrange
        var createRequest = new CreateToDoItemRequest(
            "Item To Update",
            "Description",
            Priority.Medium,
            null);

        var createResponse = await client.PostAsJsonAsync(BaseUrl, createRequest);
        var createdApiResponse = await DeserializeResponseAsync<ToDoItem>(createResponse);
        createdApiResponse.Should().NotBeNull();
        var createdResult = createdApiResponse ?? throw new InvalidOperationException("Response was null");
        createdResult.Data.Should().NotBeNull();
        var createdData = createdResult.Data ?? throw new InvalidOperationException("Data was null");
        var createdItemId = createdData.Id;

        var updateRequest = new UpdateToDoItemRequest(
            "",
            "Updated Description",
            Priority.High,
            ToDoItemStatus.InProgress,
            null);

        // Act
        var response = await client.PutAsJsonAsync($"{BaseUrl}/{createdItemId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrWhiteSpace();
    }

    [TestMethod]
    public async Task Update_ShouldReturn404_WhenItemDoesNotExist()
    {
        // Arrange
        var nonExistentId = 99999;

        var updateRequest = new UpdateToDoItemRequest(
            "Updated Title",
            "Updated Description",
            Priority.High,
            ToDoItemStatus.InProgress,
            null);

        // Act
        var response = await client.PutAsJsonAsync($"{BaseUrl}/{nonExistentId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var apiResponse = await DeserializeResponseAsync<object>(response);

        apiResponse.Should().NotBeNull();
        var result = apiResponse ?? throw new InvalidOperationException("Response was null");
        result.Success.Should().BeFalse();
        result.Error.Should().NotBeNullOrWhiteSpace();
    }

    [TestMethod]
    public async Task Delete_ShouldReturn204_WhenItemExists()
    {
        // Arrange
        var createRequest = new CreateToDoItemRequest(
            "Item To Delete",
            null,
            Priority.Low,
            null);

        var createResponse = await client.PostAsJsonAsync(BaseUrl, createRequest);
        var createdApiResponse = await DeserializeResponseAsync<ToDoItem>(createResponse);
        createdApiResponse.Should().NotBeNull();
        var createdResult = createdApiResponse ?? throw new InvalidOperationException("Response was null");
        createdResult.Data.Should().NotBeNull();
        var createdData = createdResult.Data ?? throw new InvalidOperationException("Data was null");
        var createdItemId = createdData.Id;

        // Act
        var response = await client.DeleteAsync($"{BaseUrl}/{createdItemId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify item is actually removed
        var getResponse = await client.GetAsync($"{BaseUrl}/{createdItemId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [TestMethod]
    public async Task Delete_ShouldReturn404_WhenItemDoesNotExist()
    {
        // Arrange
        var nonExistentId = 99999;

        // Act
        var response = await client.DeleteAsync($"{BaseUrl}/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var apiResponse = await DeserializeResponseAsync<object>(response);

        apiResponse.Should().NotBeNull();
        var result = apiResponse ?? throw new InvalidOperationException("Response was null");
        result.Success.Should().BeFalse();
        result.Error.Should().NotBeNullOrWhiteSpace();
    }

    private static async Task<ApiResponse<T>?> DeserializeResponseAsync<T>(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<ApiResponse<T>>(content, JsonOptions);
    }
}
