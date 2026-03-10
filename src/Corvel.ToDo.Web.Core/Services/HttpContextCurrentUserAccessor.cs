using System.Security.Claims;
using Corvel.ToDo.Abstractions.Exceptions;
using Corvel.ToDo.Abstractions.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Corvel.ToDo.Web.Core.Services;

public class HttpContextCurrentUserAccessor(
    IHttpContextAccessor httpContextAccessor) : ICurrentUserAccessor
{
    public virtual int UserId
    {
        get
        {
            var claim = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);

            if (claim is null || !int.TryParse(claim.Value, out var userId))
            {
                throw new AuthenticationFailedException("User is not authenticated.");
            }

            return userId;
        }
    }
}
