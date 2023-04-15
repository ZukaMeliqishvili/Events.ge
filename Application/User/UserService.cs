using Application.User.Model;
using Domain.User;
using Infrastructure;
using Mapster;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Application.User
{
    public class UserService:IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;
        public UserService(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }
        public async Task<List<UserResponseModel>> GetAllAsync(CancellationToken cancellationToken)
        {
            List<ApplicationUser> users = await _unitOfWork.User.GetAllAsync(cancellationToken);
            return users.Adapt<List<UserResponseModel>>();
        }

        public async Task<UserResponseModel> GetAsync(CancellationToken cancellationToken, string userId)
        {
            ApplicationUser user = await this._unitOfWork.User.GetByIdAsync(cancellationToken, userId);
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
    }
}
