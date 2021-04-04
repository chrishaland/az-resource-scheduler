using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Service.Accounts
{
    [Route("api/account/get")]
    public class GetAccountInformationHandler : QueryHandlerBase<GetAccountInformationRequest, GetAccountInformationResponse>
    {
        public override async Task<ActionResult<GetAccountInformationResponse>> Execute([FromBody] GetAccountInformationRequest request, CancellationToken ct)
        {
            await Task.CompletedTask;

            var givenName = User.FindFirst(ClaimTypes.GivenName);
            var surName = User.FindFirst(ClaimTypes.Surname);

            var dto = new AccountDto(
                GivenName: User.FindFirst(ClaimTypes.GivenName)?.Value,
                SurName: User.FindFirst(ClaimTypes.Surname)?.Value,
                Email: User.FindFirst(ClaimTypes.Email)?.Value,
                Roles: User.FindAll(ClaimTypes.Role)?.Select(claim => claim.Value).ToArray()
            );

            return Ok(new GetAccountInformationResponse(dto));
        }
    }
}
