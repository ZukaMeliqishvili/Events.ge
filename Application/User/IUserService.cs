using Application.User.Model;

namespace Application.User
{
    public interface IUserService
    {
        Task<List<UserResponseModel>> GetAllAsync(CancellationToken cancellationToken);
        Task<UserResponseModel> GetAsync(CancellationToken cancellationToken, string userId);
        Task<List<UserResponseModel>> GetNormalUsers(CancellationToken cancellationToken);
        Task GiveModerator(string userId, CancellationToken cancellationToken);
        Task<string> Login(UserloginModel model, CancellationToken cancellationToken);
        Task Regiseter(UserRegisterModel model, CancellationToken cancellationToken);
    }
}
