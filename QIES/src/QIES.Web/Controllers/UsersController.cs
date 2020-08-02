using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QIES.Api.Models;
using QIES.Core.Services;
using QIES.Core.Users;

using static System.Net.Mime.MediaTypeNames;

namespace QIES.Web.Controllers
{
    [ApiController]
    [Consumes(Application.Json)]
    [Produces(Application.Json)]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ILogger logger;
        private ILoginService loginService;
        private ILogoutService logoutService;

        public UsersController(
                ILogger<UsersController> logger,
                ILoginService loginService,
                ILogoutService logoutService)
        {
            this.logger = logger;
            this.loginService = loginService;
            this.logoutService = logoutService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<User>> Login(LoginRequest request)
        {
            var login = request.Login switch
            {
                "planner"   => LoginType.Planner,
                "agent"     => LoginType.Agent,
                _           => LoginType.None
            };

            if (login == LoginType.None)
            {
                return BadRequest();
            }

            var user = await loginService.DoLogin(login);

            return Ok(user);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout(LogoutRequest request)
        {
            var success = await logoutService.DoLogout(request.Id);
            if (success)
            {
                return Ok();
            }
            return BadRequest();
        }
    }
}
