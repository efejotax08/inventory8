using Microsoft.AspNetCore.Mvc;

namespace inventory8.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CallbackController : Controller
    {
        [HttpGet]
        public IActionResult Get([FromQuery] string oauth_token, [FromQuery] string oauth_verifier)
        {
            // Aquí puedes registrar temporalmente los valores para probar
            return Ok(new
            {
                Message = "OAuth callback received",
                Token = oauth_token,
                Verifier = oauth_verifier
            });
        }
    }
}
