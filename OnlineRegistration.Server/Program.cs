using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using OnlineRegistration.Server.Data;
using OnlineRegistration.Server.Models;
using OnlineRegistration.Server.Services;
using OnlineRegistration.Server.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ------------------ Services ------------------

// Users Table
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Personal Data Sheet
builder.Services.AddDbContext<EmployeesDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IEmailQueue, EmailBackgroundQueue>();
builder.Services.AddHostedService<EmailSender>();

// For password hashing
builder.Services.AddScoped<IPasswordHasher<Users>, PasswordHasher<Users>>();

// AFIS queue
builder.Services.AddSingleton<AfisQueueService>(); // Service shared with controller
builder.Services.AddHostedService<AfisQueueWorker>(); // Background worker

// CORS for local dev
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevPolicy", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

// ------------------ Auth (JWT + Single Session) ------------------
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtSettings = builder.Configuration.GetSection("Jwt");
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]))
    };

    // 🔑 Custom validation to enforce single active session
    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = async context =>
        {
            var db = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();

            var userId = context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                context.Fail("Invalid token");
                return;
            }

            var user = await db.Users.FindAsync(int.Parse(userId));
            if (user == null)
            {
                context.Fail("User not found");
                return;
            }

            // ✅ Compare against Authorization header (raw string from request)
            var requestToken = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (string.IsNullOrEmpty(user.ActiveToken) || user.ActiveToken != requestToken)
            {
                context.Fail("Session expired");
            }
        },
        OnAuthenticationFailed = context =>
        {
            context.NoResult();
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsJsonAsync(new { message = "Session expired" });
        }
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// ------------------ Middleware Pipeline ------------------

app.UseRouting();
app.UseCors("DevPolicy");

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Map API controllers
app.MapControllers();

// ------------------ Custom Fallback for SPA ------------------

app.MapFallback(async context =>
{
    var path = context.Request.Path;

    // If it's an API path, return 404 JSON instead of serving index.html
    if (path.StartsWithSegments("/api"))
    {
        context.Response.StatusCode = 404;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new { message = "API endpoint not found." });
        return;
    }

    // Otherwise, serve the SPA
    context.Response.ContentType = "text/html";
    await context.Response.SendFileAsync("wwwroot/index.html");
});

app.Run();
