using Application.DTOs.Chatting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MorphingTalk_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;

        public TestController(ILogger<TestController> logger)
        {
            _logger = logger;
        }

        [HttpPost("webhook-test")]
        public IActionResult TestWebhookPayload([FromBody] AIWebhookTrainingPayloadDto payload)
        {
            _logger.LogInformation("Received webhook test:");
            _logger.LogInformation($"RequestId: {payload.RequestId}");
            _logger.LogInformation($"ModelId: {payload.modelId}");
            _logger.LogInformation($"success (string): {payload.Success}");
            _logger.LogInformation($"errorMessage: {payload.ErrorMessage}");
            _logger.LogInformation($"Success (boolean): {payload.Success}");

            return Ok(new 
            {
                received = new
                {
                    requestId = payload.RequestId,
                    modelId = payload.modelId,
                    successString = payload.Success,
                    errorMessage = payload.ErrorMessage
                },
                parsed = new
                {
                    success = payload.Success,
                    errorMessage = payload.ErrorMessage
                }
            });
        }
    }
}
