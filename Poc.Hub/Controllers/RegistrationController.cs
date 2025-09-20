using Microsoft.AspNetCore.Mvc;
using Poc.Hub.Filters;
using Poc.Hub.Services;
using Shared;

namespace Poc.Hub.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RegistrationController(IClientRegistry registry, ILogger<RegistrationController> logger)
    : ControllerBase
{
    [HttpPost] 
    [HeaderAuthorization("Client-Id")]
    public async Task<ActionResult<RegistrationResponse>> Register()
    {
        var clientId =  HttpContext.Request.Headers["Client-Id"].ToString();
        
        var result = await registry.Register2ClientAsync(clientId);
        if (!result)
        {
            logger.LogWarning($"Client {clientId} failed to register.");
            
            return Conflict(new RegistrationResponse {
                Success = false,
                Error = "Client already registered"
            });
        }
    
        return Ok(new RegistrationResponse {
            Success = true,
            ClientId = clientId
        });
    }
}