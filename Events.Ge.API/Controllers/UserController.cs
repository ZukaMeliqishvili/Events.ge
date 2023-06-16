using Application.User;
using Application.User.Model;
using Events.Ge.API.Infrastructure.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;

namespace Events.Ge.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService   _userService;
        private readonly IOptions<JWTConfiguration> _options;

        public UserController(IUserService userService, IOptions<JWTConfiguration> options)
        {
            _userService = userService;
            _options = options;
        }

        [HttpPost("register")]
        public async Task Register(UserRegisterModel model, CancellationToken cancellationToken)
        {
            await _userService.Regiseter(model,cancellationToken);
        }
        [HttpPost("login")]
        public async Task<string> Login(UserloginModel model, CancellationToken cancellationToken)
        {

            string id = await _userService.Login(model, cancellationToken);
            return JWTHelper.GenerateSecurityToken(id, _options);
        }
    }
}
