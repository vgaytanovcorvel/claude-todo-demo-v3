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
[Route(RouteConstants.ApiPrefix + "/" + RouteConstants.UserProfileRoute)]
[Authorize]
public class UserProfileController(
    IUserService userService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<UserProfileResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public virtual async Task<ActionResult<ApiResponse<UserProfileResponse>>> GetProfile(
        CancellationToken cancellationToken)
    {
        var user = await userService.GetProfileAsync(cancellationToken);

        return Ok(ApiResponse<UserProfileResponse>.SuccessResponse(MapToResponse(user)));
    }

    [HttpPut]
    [ProducesResponseType(typeof(ApiResponse<UserProfileResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public virtual async Task<ActionResult<ApiResponse<UserProfileResponse>>> UpdateProfile(
        [FromBody] UpdateProfileRequest request,
        CancellationToken cancellationToken)
    {
        var user = await userService.UpdateProfileAsync(request, cancellationToken);

        return Ok(ApiResponse<UserProfileResponse>.SuccessResponse(MapToResponse(user)));
    }

    [HttpPut("password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public virtual async Task<ActionResult> ChangePassword(
        [FromBody] ChangePasswordRequest request,
        CancellationToken cancellationToken)
    {
        await userService.ChangePasswordAsync(request, cancellationToken);

        return NoContent();
    }

    private UserProfileResponse MapToResponse(User user) =>
        new(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            user.CreatedAtUtc,
            user.UpdatedAtUtc);
}
