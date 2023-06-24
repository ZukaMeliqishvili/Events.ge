using Application.User.Model;
using Domain.User;
using Infrastructure;
using Mapster;
using Microsoft.AspNetCore.Identity;


namespace Application.User
{
    public class UserService:IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        public UserService(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public async Task<List<UserResponseModel>> GetAllAsync(CancellationToken cancellationToken)
        {
            List<ApplicationUser> users = await _unitOfWork.User.GetAllAsync(cancellationToken);
            return users.Adapt<List<UserResponseModel>>();
        }

        public async Task<UserResponseModel> GetAsync(CancellationToken cancellationToken, string userId)
        {
            ApplicationUser user = await _unitOfWork.User.GetByIdAsync(cancellationToken, userId);
            if (user is null)
            {
                throw new Exception("User not found");
            }
            return user.Adapt<UserResponseModel>();
        }

        public async Task GiveModerator(string userId, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new Exception("User not found");
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Count > 0)
            {
                foreach (var role in roles)
                {
                    await _userManager.RemoveFromRoleAsync(user, role);
                }
            }
            await _userManager.AddToRoleAsync(user, "Moderator");
        }

        public async Task<List<UserResponseModel>> GetNormalUsers(CancellationToken cancellationToken)
        {
            var users = await _userManager.GetUsersInRoleAsync("User");
            return users.Adapt<List<UserResponseModel>>();
        }
        public async Task Regiseter(UserRegisterModel model, CancellationToken cancellationToken)
        {
            bool isUnique = await _unitOfWork.User.Exists(cancellationToken, x => x.UserName == model.UserName || x.Email == model.Email);
            if (isUnique)
                throw new Exception("User is already registered by given username");
            var applicationUser = new ApplicationUser();
            applicationUser.UserName = model.UserName;
            applicationUser.Email = model.Email;
            var result = await _userManager.CreateAsync(applicationUser, model.Password);
            if (!result.Succeeded)
            {
                throw new Exception("There was some problem with registering your account");
            }
            else
            {
                await _userManager.AddToRoleAsync(applicationUser, "User");
            }
        }
        public async Task<string> Login(UserloginModel model, CancellationToken cancellationToken)
        {
            SignInResult result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, false, false);
            if (!result.Succeeded)
                throw new Exception("Invalid login attempt");
            IdentityUser user = await _userManager.FindByNameAsync(model.Username);
            return user.Id;
        }

    }
}
