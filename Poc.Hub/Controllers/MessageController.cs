using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Poc.Hub.Hub;
using Poc.Hub.Mapping;

namespace Poc.Hub.Controllers;

[ApiController]
[Route("api/message")]
public class MessageController(
    IHubContext<LogHub> hubContext,
    IConnectionMapping connectionMapping,
    ILogger<MessageController> logger) : ControllerBase
{
    [HttpPost("{clientId}")]
    public async Task<IActionResult> SendToClient(string clientId, [FromBody] string message)
    {
        var connectionId = await connectionMapping.GetConnectionId(clientId);
        if (connectionId == null) return NotFound("Client not connected.");
       
        await hubContext.Clients.Client(connectionId).SendAsync("ReceiveMessage", message);
        logger.LogInformation($"Client {clientId} sent message {message}");
        
        return Ok("Message sent.");
    }
}