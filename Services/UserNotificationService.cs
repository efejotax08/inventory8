using inventory8.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using System;

namespace inventory8.Services
{
    public class FCMTokenDTO
    {
        public string FcmToken { get; set; } = string.Empty;
    }

    public class UserNotificationService
    {
        private readonly InventoryContext _context;

        public UserNotificationService(InventoryContext context)
        {
            _context = context;
        }

        public async Task<bool> ActualizarFcmTokenAsync(string uniqueIdentifier, string fcmToken)
        {
            var user = await _context.Users
             .FirstOrDefaultAsync(u => u.UniqueIdentifier == uniqueIdentifier);
            if (user == null)
                return false;

            user.FcmToken = fcmToken;
            await _context.SaveChangesAsync();

            return true;
        }
    }

}
