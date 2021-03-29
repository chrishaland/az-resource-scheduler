using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Claims;

namespace Service.Authentication
{
    [Authorize(AuthenticationSchemes = OpenIdConnectDefaults.AuthenticationScheme)]
    [Route("api/account/login")]
    public class LoginCommandHandler : Controller
    {
        private readonly ILogger<LoginCommandHandler> _logger;

        public LoginCommandHandler(ILogger<LoginCommandHandler> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = OpenIdConnectDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Execute([FromQuery] string returnUrl, CancellationToken ct)
        {
            var userId =  User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userEmail =  User.FindFirstValue(ClaimTypes.Email);
            
            _logger.LogInformation("User logged in. Id: '{UserId}', E-mail: '{UserEmail}'.", userId, userEmail);
            
            await Task.CompletedTask;
            return LocalRedirect(returnUrl ?? "/");
        }
    }
}
