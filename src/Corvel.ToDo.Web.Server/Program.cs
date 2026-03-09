using Corvel.ToDo.Web.Core.Controllers;
using Corvel.ToDo.Web.Core.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddImplementationServices();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddWebCoreServices();

builder.Services.AddControllers()
    .AddApplicationPart(typeof(ToDoItemsController).Assembly);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? [])
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
app.UseHttpsRedirection();
app.UseRouting();
app.UseCors();
app.MapControllers();

app.Run();

public partial class Program { }
