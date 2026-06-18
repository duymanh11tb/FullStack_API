using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLDA_PCCV.Infrastructure.Persistence;

namespace QLDA_PCCV.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ActivityLogController : ControllerBase
{
    private readonly AppDbContext _context;

    public ActivityLogController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("project/{projectId}")]
    public async Task<IActionResult> GetLogsByProject(Guid projectId)
    {
        var logs = await _context.ActivityLogs
            .Include(l => l.User)
            .Where(l => l.ProjectId == projectId)
            .OrderByDescending(l => l.CreatedAt)
            .Select(l => new
            {
                l.Id,
                l.UserId,
                UserFullName = l.User != null ? l.User.FullName : "Unknown",
                UserUsername = l.User != null ? l.User.Username : "unknown",
                l.TaskId,
                l.ProjectId,
                l.Action,
                ActionName = l.Action.ToString(),
                l.Detail,
                l.CreatedAt
            })
            .ToListAsync();

        return Ok(logs);
    }

    [HttpGet("task/{taskId}")]
    public async Task<IActionResult> GetLogsByTask(Guid taskId)
    {
        var logs = await _context.ActivityLogs
            .Include(l => l.User)
            .Where(l => l.TaskId == taskId)
            .OrderByDescending(l => l.CreatedAt)
            .Select(l => new
            {
                l.Id,
                l.UserId,
                UserFullName = l.User != null ? l.User.FullName : "Unknown",
                UserUsername = l.User != null ? l.User.Username : "unknown",
                l.TaskId,
                l.ProjectId,
                l.Action,
                ActionName = l.Action.ToString(),
                l.Detail,
                l.CreatedAt
            })
            .ToListAsync();

        return Ok(logs);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetLogsByUser(Guid userId)
    {
        var logs = await _context.ActivityLogs
            .Include(l => l.User)
            .Where(l => l.UserId == userId)
            .OrderByDescending(l => l.CreatedAt)
            .Select(l => new
            {
                l.Id,
                l.UserId,
                UserFullName = l.User != null ? l.User.FullName : "Unknown",
                UserUsername = l.User != null ? l.User.Username : "unknown",
                l.TaskId,
                l.ProjectId,
                l.Action,
                ActionName = l.Action.ToString(),
                l.Detail,
                l.CreatedAt
            })
            .ToListAsync();

        return Ok(logs);
    }
}
