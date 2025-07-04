using Google;
using inventory8.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace inventory8.Services
{
    public class InventoryAlertService
    {
        private readonly InventoryContext _context;
        private readonly FirebaseNotificationService _notificacionService;
        private readonly ILogger<InventoryMonitorService> _logger;

        public InventoryAlertService(InventoryContext context, FirebaseNotificationService usernotificacionService, ILogger<InventoryMonitorService> logger)
        {
            _context = context;
            _notificacionService = usernotificacionService;
            _logger = logger;
        }

        public async Task RevisarYNotificarAsync()
        {
            // Obtener productos con stock por debajo del umbral
            var productosBajos = await _context.Products
                .Where(p => p.SubscribeToInventory && p.StockQuantity <= p.LowStockThreshold)
                .Include(p => p.Supplier)
                .ToListAsync();
            _logger.LogInformation("⏱️ Productos Encontrados");
            _logger.LogInformation(productosBajos.Count.ToString());
            if (!productosBajos.Any()) return;

            // Obtener usuarios que recibirán notificación
            var usuarios = await _context.Users
                .Where(u => u.Settings != null && u.Settings.Contains("\"notifications\":"))
                .ToListAsync();
            _logger.LogInformation("⏱️ Usuarios Encontrados");
            _logger.LogInformation(usuarios.Count.ToString());
            foreach (var producto in productosBajos)
            {
                foreach (var usuario in usuarios)
                {
                    // Extraer FCM token del usuario (deberías tenerlo almacenado en la tabla `users`)
                    var fcmToken = usuario.FcmToken; // Asegúrate que existe esta propiedad
                    if (string.IsNullOrEmpty(fcmToken)) continue;

                    string titulo = "⚠️ Alerta de inventario bajo";
                    string cuerpo = $"El producto \"{producto.Name}\" está por debajo del nivel mínimo ({producto.StockQuantity} unidades).";

                    try
                    {
                        await _notificacionService.EnviarNotificacionAsync(titulo, cuerpo, fcmToken);
                    }
                    catch (Exception ex)
                    {
                        // loguear si es necesario
                    }
                }
            }
        }
    }

}
