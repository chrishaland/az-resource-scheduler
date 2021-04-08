using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Unleash;

namespace Service.Accounts
{
    [Route("api/account/get")]
    public class GetAccountInformationHandler : QueryHandlerBase<GetAccountInformationRequest, GetAccountInformationResponse>
    {
        private readonly IUnleash _featureFlagHandler;

        public GetAccountInformationHandler(IUnleash featureFlagHandler)
        {
            _featureFlagHandler = featureFlagHandler;
        }

        public override async Task<ActionResult<GetAccountInformationResponse>> Execute([FromBody] GetAccountInformationRequest request, CancellationToken ct)
        {
            await Task.CompletedTask;

            var givenName = User.FindFirst(ClaimTypes.GivenName);
            var surName = User.FindFirst(ClaimTypes.Surname);

            var dto = new AccountDto(
                GivenName: User.FindFirst(ClaimTypes.GivenName)?.Value,
                SurName: User.FindFirst(ClaimTypes.Surname)?.Value,
                Email: User.FindFirst(ClaimTypes.Email)?.Value,
                Roles: User.FindAll(ClaimTypes.Role)?.Select(claim => claim.Value).ToArray(),
                FeatureFlags: GetActiveFeatureFlags()
            );

            return Ok(new GetAccountInformationResponse(dto));
        }
        
        private string[] GetActiveFeatureFlags()
        {
            var activeFeatureFlags = new List<string>();
            var featureFlags = Enum.GetValues<FeatureFlags>().Select(f => f.ToString());

            foreach(var featureFlag in featureFlags)
            {
                //TODO Investigate setup
                if (_featureFlagHandler.IsEnabled(featureFlag))
                {
                    activeFeatureFlags.Add(featureFlag);
                }
            }

            return activeFeatureFlags.ToArray();
        }
    }
}
