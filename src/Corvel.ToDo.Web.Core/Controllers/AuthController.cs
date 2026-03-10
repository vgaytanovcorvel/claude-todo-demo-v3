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
using Microsoft.AspNetCore.RateLimiting;

namespace Corvel.ToDo.Web.Core.Controllers;

[ApiController]
[Route(RouteConstants.ApiPrefix + "/" + RouteConstants.AuthRoute)]
[AllowAnonymous]
[EnableRateLimiting(RouteConstants.AuthRateLimitPolicy)]
public class AuthController(
    IUserService userService) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<AuthTokenResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public virtual async Task<ActionResult<ApiResponse<AuthTokenResponse>>> Register(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var authToken = await userService.RegisterAsync(request, cancellationToken);

        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<AuthTokenResponse>.SuccessResponse(MapToResponse(authToken), HttpStatusCode.Created));
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<AuthTokenResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public virtual async Task<ActionResult<ApiResponse<AuthTokenResponse>>> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var authToken = await userService.LoginAsync(request, cancellationToken);

        return Ok(ApiResponse<AuthTokenResponse>.SuccessResponse(MapToResponse(authToken)));
    }

    private AuthTokenResponse MapToResponse(AuthToken token) => new(token.Token, token.ExpiresAtUtc);
}
