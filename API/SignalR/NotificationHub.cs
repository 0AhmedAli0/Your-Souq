using System.Collections.Concurrent;
using System.Security.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    [Authorize]
    public class NotificationHub : Hub
    {
        //signalR تقوم بتتبع معرف اتصال العميل فقط وهذا ما يستخدمه المتصفح للحفاظ علي الاتصال مع خدمه signalR

        private static readonly ConcurrentDictionary<string, string> UserConnections = new();
        public override Task OnConnectedAsync()
        {//هذه الطريه تمكننا من ارسال اشعارات خارج التطبيق
            // عند اتصال العميل، نقوم بتخزين معرف الاتصال الخاص به في القاموس
            var email = Context.User?.FindFirstValue(ClaimTypes.Email) ?? throw new AuthenticationException("Email claim not found");
            if (!string.IsNullOrEmpty(email)) UserConnections[email] = Context.ConnectionId;

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            // عند قطع الاتصال، نقوم بإزالة معرف الاتصال من القاموس
            var email = Context.User?.FindFirstValue(ClaimTypes.Email) ?? throw new AuthenticationException("Email claim not found");
            if (!string.IsNullOrEmpty(email)) UserConnections.TryRemove(email, out _);

            return base.OnDisconnectedAsync(exception);
        }

        public static string? GetConnectionIdByEmail(string email)
        {
            // نحاول الحصول على معرف الاتصال بناءً على البريد الإلكتروني
            UserConnections.TryGetValue(email, out var connectionId);
            return connectionId;
        }
    }
}
