using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace inventory8.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        // /auth/login
        [HttpGet("login")]
        public IActionResult Login(string returnUrl = "/")
        {
            var properties = new AuthenticationProperties { RedirectUri = returnUrl };
            return Challenge(properties, "CleverCloud");
        }

        // Esta ruta es el callback configurado en OAuth (ejemplo /signin-clevercloud)
        // No necesitas acción aquí explícita, lo maneja middleware, pero puedes agregar para debug

        // /auth/profile
        [Authorize]
        [HttpGet("profile")]
        public IActionResult Profile()
        {
            var user = User;
            if (user.Identity?.IsAuthenticated != true)
                return Unauthorized();

            return Ok(new
            {
                Name = user.Identity.Name,
                Claims = user.Claims.Select(c => new { c.Type, c.Value })
            });
        }

        // /auth/logout
        [HttpGet("logout")]
        public IActionResult Logout()
        {
            return SignOut(new AuthenticationProperties { RedirectUri = "/" },
                CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
