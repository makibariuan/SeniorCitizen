using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OnlineRegistration.Server.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineRegistration.Server.Middleware
{
    public class TokenValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, AppDbContext db)
        {
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();

            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();

                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var jwt = handler.ReadJwtToken(token);
                    var userId = jwt.Claims.FirstOrDefault(c => c.Type == "id")?.Value;

                    if (!string.IsNullOrEmpty(userId))
                    {
                        var user = await db.Users
                            .AsNoTracking()
                            .FirstOrDefaultAsync(u => u.Id.ToString() == userId);

                        if (user == null || user.ActiveToken != token)
                        {
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            await context.Response.WriteAsJsonAsync(new { message = "Session expired or logged out" });
                            return;
                        }
                    }
                }
                catch
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsJsonAsync(new { message = "Invalid token" });
                    return;
                }
            }

            await _next(context);
        }
    }
}
