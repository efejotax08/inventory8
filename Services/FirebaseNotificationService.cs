using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;

namespace inventory8.Services
{
    public class FirebaseNotificationService
    {
        private readonly FirebaseMessaging _messaging;

        public FirebaseNotificationService(IWebHostEnvironment env)
        {
            // Evita inicialización múltiple
            if (FirebaseApp.DefaultInstance != null)
            {
                _messaging = FirebaseMessaging.DefaultInstance;
                return;
            }
            GoogleCredential credential;

            if (env.IsDevelopment())
            {
                // 🔹 Cargar desde archivo local en desarrollo
                var jsonPath = Path.Combine(env.ContentRootPath, "Credentials", "notif_inventory8.json");

                if (!File.Exists(jsonPath))
                    throw new FileNotFoundException($"Archivo de credenciales no encontrado: {jsonPath}");

                credential = GoogleCredential.FromFile(jsonPath);
            }
            else
            {
                // 🔹 Leer desde variable de entorno en producción
                var firebaseJson = Environment.GetEnvironmentVariable("FIREBASE_CONFIG");

                if (string.IsNullOrWhiteSpace(firebaseJson))
                    throw new InvalidOperationException("La variable de entorno 'FIREBASE_CONFIG' no está configurada.");

                credential = GoogleCredential.FromJson(firebaseJson);
            }
            FirebaseApp.Create(new AppOptions
            {
                Credential = credential
            });

            _messaging = FirebaseMessaging.DefaultInstance;

        }

        public async Task<string> EnviarNotificacionAsync(string titulo, string cuerpo, string fcmToken)
        {
            if (string.IsNullOrEmpty(fcmToken))
                throw new ArgumentException("El FCM Token no puede ser nulo o vacío.", nameof(fcmToken));

            var mensaje = new Message
            {
                Token = fcmToken,
                Notification = new Notification
                {
                    Title = titulo,
                    Body = cuerpo
                },
                Data = new Dictionary<string, string>
        {
            { "tipo", "alerta" },
            { "origen", "backend-dotnet" }
        },
                Android = new AndroidConfig
                {
                    Priority = Priority.High,
                    Notification = new AndroidNotification
                    {
                        Sound = "default"
                    }
                },
                Apns = new ApnsConfig
                {
                    Aps = new Aps
                    {
                        Sound = "default"
                    }
                }
            };

            try
            {
                return await _messaging.SendAsync(mensaje);
            }
            catch (FirebaseMessagingException ex)
            {
                // Puedes loguear más detalles si lo necesitas
                Console.WriteLine($"Error al enviar notificación: {ex.Message}");
                throw;
            }
        }

    }
}
