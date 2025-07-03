using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;

namespace inventory8.Services
{
    public class FirebaseNotificationService
    {
        private readonly FirebaseMessaging _messaging;

        public FirebaseNotificationService()
        {
            // Obtener el JSON desde la variable de entorno
            var firebaseJson = Environment.GetEnvironmentVariable("FIREBASE_CONFIG");

            if (string.IsNullOrWhiteSpace(firebaseJson))
            {
                throw new InvalidOperationException("La variable de entorno 'FIREBASE_CONFIG' no está configurada.");
            }

            // Inicializar la app de Firebase si no está ya inicializada
            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions
                {
                    Credential = GoogleCredential.FromJson(firebaseJson)
                });
            }

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
