using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace inventory8.Services
{
    public class InventoryMonitorService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<InventoryMonitorService> _logger;

        public InventoryMonitorService(IServiceProvider serviceProvider, ILogger<InventoryMonitorService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("⏱️ InventoryMonitorService iniciado");

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("⏱️ Nuevo proceso");
                using (var scope = _serviceProvider.CreateScope())
                {
                    var alertService = scope.ServiceProvider.GetRequiredService<InventoryAlertService>();

                    try
                    {
                        await alertService.RevisarYNotificarAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "❌ Error al verificar inventario bajo.");
                    }
                }

                // Esperar 1 hora (puedes ajustar este tiempo)
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}
