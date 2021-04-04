using Hangfire.Dashboard;
using System.Security.Claims;

namespace Host.Authorizations
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            var adminRole = httpContext.User.FindFirst(c =>
                c.Type == ClaimTypes.Role &&
                c.Value == "admin"
            );

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            return httpContext.User.Identity.IsAuthenticated && adminRole != null;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }
    }
}
