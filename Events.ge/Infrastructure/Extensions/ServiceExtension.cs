using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Persistance.Context;
using Persistance.Seeding;
using Infrastructure;
using Application.User;
using Application._Event;
using Application._Ticket;

namespace Events.ge.Infrastructure.Extensions
{
    public static class ServiceExtension
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ITicketService,TicketService>();
            services.AddScoped<IEventService, EventService>();
            services.AddScoped<IdbInitializer, DbInitializer>();
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
        }
    }
}
