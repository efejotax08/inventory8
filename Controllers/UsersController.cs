using inventory8.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace inventory8.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;

        public UsersController(UserService userService)
        {
            _userService = userService;
        }


        [Authorize]
        [HttpPost("fcm-token")]
        public async Task<IActionResult> GuardarFcmToken( [FromBody] FCMTokenDTO dto)
        {
            var uniqueIdentifier = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var nombre = User.Identity?.Name; // si usaste ClaimTypes.Name
            var exito = await _userService.ActualizarFcmTokenAsync(uniqueIdentifier, dto.FcmToken);
            if (!exito)
                return NotFound(new { message = "Usuario no encontrado" });

            return Ok(new { message = "Token FCM actualizado correctamente" });
        }
    }

}
