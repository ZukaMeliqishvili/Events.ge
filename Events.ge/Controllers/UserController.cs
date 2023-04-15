using Application.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Events.ge.Controllers
{
    [Authorize(Roles ="Admin")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var users = await _userService.GetNormalUsers(cancellationToken);
            return View(users);
        }
        public async Task<IActionResult> MakeModerator(string id, CancellationToken cancellationToken)
        {
            try
            {
                await _userService.GiveModerator(id, cancellationToken);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}
