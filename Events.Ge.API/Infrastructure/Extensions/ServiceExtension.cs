using Application._Event;
using Application._Ticket;
using Application.User;
using Infrastructure;
using Microsoft.AspNetCore.Identity;
using Persistance.Context;
using Persistance.Seeding;

namespace Events.Ge.API.Infrastructure.Extensions
{
    public static class ServiceExtension
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ITicketService, TicketService>();
            services.AddScoped<IEventService, EventService>();
            services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
        }
    }
}
