using Corvel.ToDo.Web.Core.Controllers;
using Corvel.ToDo.Web.Core.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddImplementationServices(builder.Configuration);
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddWebCore();

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

builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddApiRateLimiting();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseGlobalExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

// Enables WebApplicationFactory<Program> for integration tests
public partial class Program { }
