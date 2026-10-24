using Microsoft.AspNetCore.Mvc;
using StudentManagement.Api.Services;

namespace StudentManagement.Api.Controllers;

public class AskRequest
{
    public string SessionId { get; set; } = string.Empty;
    public string Question { get; set; } = string.Empty;
}

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;
    private readonly IIndexingService _indexingService;

    public ChatController(IChatService chatService, IIndexingService indexingService)
    {
        _chatService = chatService;
        _indexingService = indexingService;
    }

    [HttpPost("ask")]
    public async Task<IActionResult> Ask([FromBody] AskRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Question))
            return BadRequest(new { answer = "Question is required." });

        // If no session id was supplied, start a new one.
        var sessionId = string.IsNullOrWhiteSpace(request.SessionId)
            ? Guid.NewGuid().ToString()
            : request.SessionId;

        try
        {
            var answer = await _chatService.AskAsync(sessionId, request.Question);
            return Ok(new { answer, sessionId });
        }
        catch (Exception ex)
        {
            return Ok(new { answer = $"The assistant couldn't answer right now: {ex.Message}", sessionId });
        }
    }

    [HttpPost("reindex")]
    public async Task<IActionResult> Reindex()
    {
        try
        {
            await _indexingService.RebuildIndexAsync();
            return Ok(new { message = "Knowledge base rebuilt." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }
}