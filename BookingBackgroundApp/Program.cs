using Application._Ticket;
using BookingBackgroundApp;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Persistance.Context;
using Serilog;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json")
    .Build();
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService()
    .ConfigureServices(services =>
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        services.AddScoped<ITicketService, TicketService>();
        services.AddScoped<IUnitOfWork,UnitOfWork>();
        services.AddScoped<ServiceClient>();
        services.AddHostedService<Worker>();
    }).UseSerilog()
    .Build();
try
{
    await host.RunAsync();
}
catch(Exception ex)
{
    Log.Error("Application Crashed");
}
finally
{
    await Log.CloseAndFlushAsync();
}
