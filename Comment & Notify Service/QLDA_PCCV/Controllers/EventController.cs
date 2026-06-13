using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLDA_PCCV.Services;

namespace QLDA_PCCV.Controllers;

[Authorize]
[ApiController]
[Route("api/events")]
public class EventController : ControllerBase
{
    private readonly INotificationEventService _notificationEventService;

    public EventController(INotificationEventService notificationEventService)
    {
        _notificationEventService = notificationEventService;
    }

    [HttpPost("consume")]
    public async Task<IActionResult> Consume([FromBody] NotificationEventRequest request, CancellationToken cancellationToken)
    {
        var result = await _notificationEventService.ConsumeAsync(request, cancellationToken);
        if (!result.IsValid)
        {
            return BadRequest(new { result.Message });
        }

        return Ok(new
        {
            result.Message,
            result.CreatedNotifications
        });
    }
}
