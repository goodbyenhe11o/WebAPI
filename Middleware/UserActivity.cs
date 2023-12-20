using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace igs_backend.Middleware
{
    public class UserActivity
    {
        private readonly RequestDelegate _next;

        public UserActivity(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync (HttpContext context)
        {
            //update logged in user's last activity time
            context.Session.SetString("LastActivityTime", DateTime.UtcNow.ToString());

            await _next(context);
        }

    }
}
