using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Linq;

namespace Service.Authentication
{
    [Authorize(AuthenticationSchemes = OpenIdConnectDefaults.AuthenticationScheme, Policy = "account")]
    [Route("api/account/login")]
    public class LoginCommandHandler : Controller
    {
        private readonly ILogger<LoginCommandHandler> _logger;

        public LoginCommandHandler(ILogger<LoginCommandHandler> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult> Execute([FromQuery] string returnUrl, CancellationToken ct)
        {
            var userId =  User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userEmail =  User.FindFirstValue(ClaimTypes.Email);
            var roles =  User.FindAll(ClaimTypes.Role)?.Select(c => c.Value);

            _logger.LogInformation("User logged in. Id: '{UserId}', E-mail: '{UserEmail}'. Roles: {UserRoles}", userId, userEmail, roles);
            
            await Task.CompletedTask;
            return LocalRedirect(returnUrl ?? "/");
        }
    }
}
