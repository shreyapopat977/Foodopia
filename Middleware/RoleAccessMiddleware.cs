using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Foodopia.Middleware
{
    public class RoleAccessMiddleware
    {
        private readonly RequestDelegate _next;

        public RoleAccessMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower();
            var role = context.Session.GetString("UserRole");

            // 🚫 Protect Admin routes
            if (path.StartsWith("/admin") && role != "Admin")
            {
                context.Response.Redirect("/Account/Login");
                return;
            }

            // 🚫 Protect Cart/Order routes
            if ((path.StartsWith("/cart") || path.StartsWith("/order")) && role != "User")
            {
                context.Response.Redirect("/Account/Login");
                return;
            }

            await _next(context);
        }
    }
}
