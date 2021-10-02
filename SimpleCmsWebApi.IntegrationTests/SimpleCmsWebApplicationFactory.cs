using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SimpleCmsWebApi.Data;
using System.Linq;

namespace SimpleCmsWebApi.IntegrationTests
{
    public class SimpleCmsWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>
        where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<SimpleCmsDbContext>));
                services.Remove(descriptor);
                services.AddDbContext<SimpleCmsDbContext>(opt => opt.UseInMemoryDatabase("testDb"));

                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<SimpleCmsDbContext>();
                    db.Database.EnsureCreated();
                    DbUtilities.InitializeDbForTests(db);
                }
            });
        }
    }
}
