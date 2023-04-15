using Domain.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistance.Context;

namespace Persistance.Seeding
{
    public class DbInitializer : IdbInitializer
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;

        public DbInitializer(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
        }

        public void Initialize()
        {
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch
            {

            }

            if (!_roleManager.RoleExistsAsync("Admin").GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole("Admin")).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole("Moderator")).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole("User")).GetAwaiter().GetResult();
                ApplicationUser user = new ApplicationUser();
                user.UserName = "Administrator";
                user.NormalizedUserName = user.UserName.ToUpper();
                user.Email = "admin@gmail.com";
                user.NormalizedEmail = user.Email.ToUpper();
                _userManager.CreateAsync(user, "Admin#123").GetAwaiter().GetResult();
                ApplicationUser applicationUser = _db.ApplicationUsers.FirstOrDefault(x => x.Email == user.Email);
                _userManager.AddToRoleAsync(applicationUser, "Admin").GetAwaiter().GetResult();
            }

        }
    }
}
