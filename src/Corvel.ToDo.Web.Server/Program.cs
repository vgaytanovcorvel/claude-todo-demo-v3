using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.RateLimiting;
using Corvel.ToDo.Common.Constants;
using Corvel.ToDo.Common.Dtos;
using Corvel.ToDo.Implementation.Options;
using Microsoft.AspNetCore.RateLimiting;
using Corvel.ToDo.Web.Core.Controllers;
using Corvel.ToDo.Web.Core.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddImplementationServices(builder.Configuration);
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddWebCoreServices();

builder.Services.AddControllers()
    .AddApplicationPart(typeof(ToDoItemsController).Assembly);

var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? [];

builder.Services.AddCors(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
    }
    else
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
    }
});

// Authentication
var jwtSection = builder.Configuration.GetSection(JwtOptions.SectionName);
var jwtKey = jwtSection["Key"]
    ?? throw new InvalidOperationException(
        "JWT signing key is not configured. Set 'Jwt:Key' via user-secrets, environment variables, or a secret manager.");

if (Encoding.UTF8.GetByteCount(jwtKey) < 32)
{
    throw new InvalidOperationException(
        "JWT signing key must be at least 32 bytes long.");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });

builder.Services.AddAuthorization();

// Rate limiting
var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.ContentType = "application/json";
        var response = ApiResponse<object>.FailureResponse(
            "Too many requests. Please try again later.",
            HttpStatusCode.TooManyRequests);
        await context.HttpContext.Response.WriteAsync(
            JsonSerializer.Serialize(response, jsonOptions),
            cancellationToken);
    };

    options.AddFixedWindowLimiter(RouteConstants.AuthRateLimitPolicy, opt =>
    {
        opt.Window = TimeSpan.FromMinutes(1);
        opt.PermitLimit = 10;
        opt.QueueLimit = 0;
    });
});

var app = builder.Build();

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
app.UseHttpsRedirection();
app.UseRouting();
app.UseCors();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }
