using System.Net;
using Corvel.ToDo.Abstractions.Interfaces;
using Corvel.ToDo.Abstractions.Models;
using Corvel.ToDo.Abstractions.Requests;
using Corvel.ToDo.Common.Constants;
using Corvel.ToDo.Common.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Corvel.ToDo.Web.Core.Controllers;

[ApiController]
[Route(RouteConstants.ApiPrefix + "/" + RouteConstants.ToDoItemsRoute)]
public class ToDoItemsController(
    IToDoItemService toDoItemService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ToDoItem>>), StatusCodes.Status200OK)]
    public virtual async Task<ActionResult<ApiResponse<IReadOnlyList<ToDoItem>>>> GetAll(
        CancellationToken cancellationToken)
    {
        var items = await toDoItemService.GetAllAsync(cancellationToken);

        return Ok(ApiResponse<IReadOnlyList<ToDoItem>>.SuccessResponse(items));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<ToDoItem>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ToDoItem>), StatusCodes.Status404NotFound)]
    public virtual async Task<ActionResult<ApiResponse<ToDoItem>>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var item = await toDoItemService.GetByIdAsync(id, cancellationToken);

        if (item is null)
        {
            return NotFound(ApiResponse<ToDoItem>.FailureResponse(
                $"ToDo item not found (Id: {id}).",
                HttpStatusCode.NotFound));
        }

        return Ok(ApiResponse<ToDoItem>.SuccessResponse(item));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ToDoItem>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public virtual async Task<ActionResult<ApiResponse<ToDoItem>>> Create(
        [FromBody] CreateToDoItemRequest request,
        CancellationToken cancellationToken)
    {
        var item = await toDoItemService.CreateAsync(request, cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = item.Id },
            ApiResponse<ToDoItem>.SuccessResponse(item, HttpStatusCode.Created));
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<ToDoItem>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public virtual async Task<ActionResult<ApiResponse<ToDoItem>>> Update(
        int id,
        [FromBody] UpdateToDoItemRequest request,
        CancellationToken cancellationToken)
    {
        var item = await toDoItemService.UpdateAsync(id, request, cancellationToken);

        return Ok(ApiResponse<ToDoItem>.SuccessResponse(item));
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public virtual async Task<ActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        await toDoItemService.DeleteAsync(id, cancellationToken);

        return NoContent();
    }
}
