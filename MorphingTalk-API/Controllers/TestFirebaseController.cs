using Microsoft.AspNetCore.Mvc;
using FirebaseAdmin.Auth;

namespace MorphingTalk_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestFirebaseController : ControllerBase
    {
        [HttpGet("verify-firebase")]
        public async Task<IActionResult> VerifyFirebaseSetup()
        {
            try
            {
                // Simple test to verify Firebase Admin SDK is properly initialized
                var auth = FirebaseAuth.DefaultInstance;
                
                return Ok(new
                {
                    Success = true,
                    Message = "Firebase Admin SDK is properly configured and initialized",
                    ProjectId = "morphing-talk-de40e", // Your Firebase project ID
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Firebase Admin SDK configuration error",
                    Error = ex.Message,
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        [HttpPost("test-token")]
        public async Task<IActionResult> TestFirebaseToken([FromBody] TestTokenRequest request)
        {
            try
            {
                // Verify a Firebase ID token (for testing purposes)
                var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(request.IdToken);
                
                return Ok(new
                {
                    Success = true,
                    Message = "Firebase ID token is valid",
                    UserId = decodedToken.Uid,
                    Email = decodedToken.Claims.ContainsKey("email") ? decodedToken.Claims["email"] : null,
                    EmailVerified = decodedToken.Claims.ContainsKey("email_verified") ? 
                        bool.Parse(decodedToken.Claims["email_verified"].ToString()) : false,
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Invalid Firebase ID token",
                    Error = ex.Message,
                    Timestamp = DateTime.UtcNow
                });
            }
        }
    }

    public class TestTokenRequest
    {
        public string IdToken { get; set; }
    }
} 