using Domain.User;
using Persistance.Context;
namespace Infrastructure.User
{
    public class UserRepository:BaseRepository<ApplicationUser>,IUserRepository
    {
        private readonly ApplicationDbContext db;

        public UserRepository(ApplicationDbContext db) : base(db)
        {
            this.db = db;
        }
    }   
}
