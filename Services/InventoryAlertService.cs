using Google;
using inventory8.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace inventory8.Services
{
    public class InventoryAlertService
    {
        private readonly InventoryContext _context;
        private readonly FirebaseNotificationService _notificacionService;

        public InventoryAlertService(InventoryContext context, FirebaseNotificationService usernotificacionService)
        {
            _context = context;
            _notificacionService = usernotificacionService;
        }

        public async Task RevisarYNotificarAsync()
        {
            // Obtener productos con stock por debajo del umbral
            var productosBajos = await _context.Products
                .Where(p => p.SubscribeToInventory && p.StockQuantity <= p.LowStockThreshold)
                .Include(p => p.Supplier)
                .ToListAsync();

            if (!productosBajos.Any()) return;

            // Obtener usuarios que recibirán notificación
            var usuarios = await _context.Users
                .Where(u => u.Settings != null && u.Settings.Contains("\"notificaciones\":"))
                .ToListAsync();

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
