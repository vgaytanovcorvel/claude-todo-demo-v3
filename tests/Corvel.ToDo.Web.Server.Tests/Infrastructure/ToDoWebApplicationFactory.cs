using System.Security.Claims;
using System.Text.Encodings.Web;
using Corvel.ToDo.Repository.Contexts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Corvel.ToDo.Web.Server.Tests.Infrastructure;

public class ToDoWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"ToDoTestDb_{Guid.NewGuid():N}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "TestOnly-SuperSecret-Key-That-Is-At-Least-32-Bytes-Long!!"
            });
        });

        builder.ConfigureServices(services =>
        {
            var descriptorsToRemove = services
                .Where(d =>
                    d.ServiceType == typeof(IDbContextFactory<ToDoDbContext>) ||
                    d.ServiceType == typeof(DbContextOptions<ToDoDbContext>) ||
                    d.ServiceType == typeof(DbContextOptions) ||
                    d.ServiceType == typeof(ToDoDbContext) ||
                    (d.ServiceType.IsGenericType &&
                     d.ServiceType.GetGenericTypeDefinition() == typeof(DbContextOptions<>)))
                .ToList();

            foreach (var descriptor in descriptorsToRemove)
            {
                services.Remove(descriptor);
            }

            var databaseName = _databaseName;

            services.AddSingleton<DbContextOptions<ToDoDbContext>>(_ =>
            {
                var optionsBuilder = new DbContextOptionsBuilder<ToDoDbContext>();
                optionsBuilder.UseInMemoryDatabase(databaseName);
                return optionsBuilder.Options;
            });

            services.AddSingleton<IDbContextFactory<ToDoDbContext>>(sp =>
            {
                var options = sp.GetRequiredService<DbContextOptions<ToDoDbContext>>();
                return new InMemoryDbContextFactory(options);
            });

            // Replace authentication with a test scheme that auto-authenticates
            services.AddAuthentication(TestAuthHandler.SchemeName)
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                    TestAuthHandler.SchemeName, _ => { });
        });
    }

    private sealed class InMemoryDbContextFactory(DbContextOptions<ToDoDbContext> options)
        : IDbContextFactory<ToDoDbContext>
    {
        public ToDoDbContext CreateDbContext()
        {
            return new ToDoDbContext(options);
        }
    }

    private sealed class TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
    {
        public const string SchemeName = "TestScheme";

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, "TestUser"),
                new Claim(ClaimTypes.Email, "test@example.com")
            };

            var identity = new ClaimsIdentity(claims, SchemeName);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, SchemeName);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
