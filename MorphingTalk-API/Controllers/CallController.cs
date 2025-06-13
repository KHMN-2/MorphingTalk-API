using Application.DTOs.Calls;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MorphingTalk_API.Hubs;
using System.Security.Claims;

namespace MorphingTalk_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CallController : ControllerBase
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly ILogger<CallController> _logger;

        public CallController(IHubContext<ChatHub> hubContext, ILogger<CallController> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        /// <summary>
        /// Initiate a call to another user
        /// </summary>
        [HttpPost("start")]
        public async Task<IActionResult> StartCall([FromBody] StartCallRequest request)
        {
            try
            {
                var callerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(callerId))
                {
                    return Unauthorized("User not authenticated");
                }

                _logger.LogInformation($"Starting {request.CallType} call from {callerId} to {request.TargetUserId} in conversation {request.ConversationId}");

                // Send call invitation through SignalR
                await _hubContext.Clients.User(request.TargetUserId)
                    .SendAsync("CallInvitation", callerId, request.ConversationId, request.CallType);

                var response = new CallStatusResponse
                {
                    ConversationId = request.ConversationId,
                    Status = "initiated",
                    Timestamp = DateTime.UtcNow,
                    Participants = new List<string> { callerId, request.TargetUserId }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting call");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Respond to a call invitation
        /// </summary>
        [HttpPost("respond")]
        public async Task<IActionResult> RespondToCall([FromBody] CallInvitationResponse request)
        {
            try
            {
                var responderId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(responderId))
                {
                    return Unauthorized("User not authenticated");
                }

                _logger.LogInformation($"Call response from {responderId} to {request.CallerId} in conversation {request.ConversationId}: {(request.Accepted ? "accepted" : "declined")}");

                // Send response through SignalR
                await _hubContext.Clients.User(request.CallerId)
                    .SendAsync("CallResponse", responderId, request.ConversationId, request.Accepted);

                var response = new CallStatusResponse
                {
                    ConversationId = request.ConversationId,
                    Status = request.Accepted ? "accepted" : "declined",
                    Timestamp = DateTime.UtcNow,
                    Participants = new List<string> { request.CallerId, responderId }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error responding to call");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// End an active call
        /// </summary>
        [HttpPost("end")]
        public async Task<IActionResult> EndCall([FromBody] EndCallRequest request)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User not authenticated");
                }

                _logger.LogInformation($"Ending call by {userId} in conversation {request.ConversationId}");

                // Notify all participants in the call group that the call has ended
                await _hubContext.Clients.Group($"call_{request.ConversationId}")
                    .SendAsync("CallEnded", userId, request.ConversationId);

                var response = new CallStatusResponse
                {
                    ConversationId = request.ConversationId,
                    Status = "ended",
                    Timestamp = DateTime.UtcNow,
                    Participants = new List<string>()
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ending call");
                return StatusCode(500, "Internal server error");
            }
        }        /// <summary>
        /// Get the status of a call in a conversation
        /// </summary>
        [HttpGet("status/{conversationId}")]
        public Task<IActionResult> GetCallStatus(string conversationId)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Task.FromResult<IActionResult>(Unauthorized("User not authenticated"));
                }

                // This is a simplified implementation
                // In a real application, you would store call states in a database or cache
                var response = new CallStatusResponse
                {
                    ConversationId = conversationId,
                    Status = "inactive",
                    Timestamp = DateTime.UtcNow,
                    Participants = new List<string>()
                };

                return Task.FromResult<IActionResult>(Ok(response));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting call status");
                return Task.FromResult<IActionResult>(StatusCode(500, "Internal server error"));
            }
        }
    }
}
