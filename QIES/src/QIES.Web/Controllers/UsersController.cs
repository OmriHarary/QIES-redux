using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QIES.Api.Models;
using QIES.Api.Responses;
using QIES.Core.Services;
using QIES.Core.Users;

using static System.Net.Mime.MediaTypeNames;

namespace QIES.Web.Controllers
{
    [ApiController]
    [Consumes(Application.Json)]
    [Produces(Application.Json)]
    [Route("api/v1/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> logger;
        private readonly ILoginService loginService;
        private readonly ILogoutService logoutService;

        public UsersController(
            ILogger<UsersController> logger,
            ILoginService loginService,
            ILogoutService logoutService)
        {
            this.logger = logger;
            this.loginService = loginService;
            this.logoutService = logoutService;
        }

        /// <summary>
        /// Start a session.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <response code="200">Successful login.</response>
        /// <response code="400">Invalid login requested.</response>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
        {
            var login = request.Login switch
            {
                "planner"   => LoginType.Planner,
                "agent"     => LoginType.Agent,
                _           => LoginType.None
            };
            logger.LogInformation("Requested login {loginRequested} resolved as type {loginResolved}", request.Login, login);
            if (login == LoginType.None)
            {
                return BadRequest();
            }

            var user = await loginService.DoLogin(login);
            var response = new LoginResponse
            {
                UserId = user.Id,
                Type = user.Type
            };

            return Ok(response);
        }

        /// <summary>
        /// End a session.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <response code="200">Successful logout.</response>
        /// <response code="400">Failed to logout.</response>
        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Logout(LogoutRequest request)
        {
            var success = await logoutService.DoLogout(request.UserId);
            if (success)
            {
                logger.LogInformation("User {id} logged out.", request.UserId);
                return Ok();
            }
            return BadRequest();
        }
    }
}
