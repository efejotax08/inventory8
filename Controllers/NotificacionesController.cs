using inventory8.Entities;
using inventory8.Services;
using Microsoft.AspNetCore.Mvc;

namespace inventory8.Controllers
{
    [ApiController]
    [Route("api/notificaciones")]
    public class NotificacionesController : ControllerBase
    {
        private readonly FirebaseNotificationService _firebase;

        public NotificacionesController(FirebaseNotificationService firebase)
        {
            _firebase = firebase;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] NotificacionDTO dto)
        {
            var id = await _firebase.EnviarNotificacionAsync(dto.Titulo, dto.Mensaje, dto.FcmToken);
            return Ok(new { id });
        }
    }

}
