using Persistance.Seeding;

namespace Events.ge.Infrastructure
{
    public static class SeedDatabase
    {
        public static void Seed(WebApplication app)
        {
            using (IServiceScope scope = app.Services.CreateScope())
                scope.ServiceProvider.GetRequiredService<IdbInitializer>().Initialize();
        }
    }
}
