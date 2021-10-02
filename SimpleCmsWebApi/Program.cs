using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SimpleCmsWebApi.Data;
using System;

namespace SimpleCmsWebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            using (var serviceScope = host.Services.CreateScope())
            {
                var config = serviceScope.ServiceProvider.GetRequiredService<IConfiguration>();
                var logger = serviceScope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                
                var ensureDBCreated = config.GetValue<bool>("EnsureDBCreated");
                if (ensureDBCreated)
                {
                    var context = serviceScope.ServiceProvider.GetRequiredService<SimpleCmsDbContext>();

                    try
                    {
                        context.Database.Migrate();
                    }
                    catch(Exception ex)
                    {
                        logger.LogError(ex, "Error during db migration");
                    }
                }
            }
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
