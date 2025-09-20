using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Poc.Hub.Filters;

public class HeaderAuthorization(string headerName) : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        string? headerValue = context.HttpContext.Request.Headers[headerName].FirstOrDefault();
        if (string.IsNullOrEmpty(headerValue))
        {
            context.Result = new ForbidResult();
        }
    }
}