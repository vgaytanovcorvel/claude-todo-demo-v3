using System.Net;
using Corvel.ToDo.Abstractions.Interfaces;
using Corvel.ToDo.Abstractions.Models;
using Corvel.ToDo.Abstractions.Requests;
using Corvel.ToDo.Common.Constants;
using Corvel.ToDo.Common.Dtos;
using Corvel.ToDo.Web.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Corvel.ToDo.Web.Core.Controllers;

[ApiController]
[Route(RouteConstants.ApiPrefix + "/" + RouteConstants.ToDoItemsRoute)]
[Authorize]
public class ToDoItemsController(
    IToDoItemService toDoItemService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ToDoItemResponse>>), StatusCodes.Status200OK)]
    public virtual async Task<ActionResult<ApiResponse<IReadOnlyList<ToDoItemResponse>>>> GetAll(
        CancellationToken cancellationToken)
    {
        var items = await toDoItemService.GetAllAsync(cancellationToken);

        return Ok(ApiResponse<IReadOnlyList<ToDoItemResponse>>.SuccessResponse(items.Select(MapToResponse).ToList()));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<ToDoItemResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ToDoItemResponse>), StatusCodes.Status404NotFound)]
    public virtual async Task<ActionResult<ApiResponse<ToDoItemResponse>>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var item = await toDoItemService.GetByIdAsync(id, cancellationToken);

        if (item is null)
        {
            return NotFound(ApiResponse<ToDoItemResponse>.FailureResponse(
                $"ToDo item not found (Id: {id}).",
                HttpStatusCode.NotFound));
        }

        return Ok(ApiResponse<ToDoItemResponse>.SuccessResponse(MapToResponse(item)));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ToDoItemResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public virtual async Task<ActionResult<ApiResponse<ToDoItemResponse>>> Create(
        [FromBody] CreateToDoItemRequest request,
        CancellationToken cancellationToken)
    {
        var item = await toDoItemService.CreateAsync(request, cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = item.Id },
            ApiResponse<ToDoItemResponse>.SuccessResponse(MapToResponse(item), HttpStatusCode.Created));
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<ToDoItemResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public virtual async Task<ActionResult<ApiResponse<ToDoItemResponse>>> Update(
        int id,
        [FromBody] UpdateToDoItemRequest request,
        CancellationToken cancellationToken)
    {
        var item = await toDoItemService.UpdateAsync(id, request, cancellationToken);

        return Ok(ApiResponse<ToDoItemResponse>.SuccessResponse(MapToResponse(item)));
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public virtual async Task<ActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        await toDoItemService.DeleteAsync(id, cancellationToken);

        return NoContent();
    }

    private ToDoItemResponse MapToResponse(ToDoItem item) =>
        new(item.Id, item.UserId, item.Title, item.Description, item.Priority, item.Status, item.CreatedAtUtc, item.UpdatedAtUtc, item.DueDate, item.CompletedAtUtc);
}
