using NUnit.Framework;
using Service.Accounts;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Tests.IntegrationTests.Accounts
{
    [TestFixture]
    public class Get_accout_information_tests
    {
        [Test]
        public async Task Get_account_information()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/account/get");

            var (response, content) = await SUT.SendHttpRequest<GetAccountInformationResponse>(request, new { });
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            Assert.Multiple(() =>
            {
                Assert.That(content.Account.GivenName, Is.EqualTo("John"));
                Assert.That(content.Account.SurName, Is.EqualTo("Doe"));
                Assert.That(content.Account.Email, Is.EqualTo("john.doe@example.com"));
                CollectionAssert.Contains(content.Account.Roles, "admin");
                CollectionAssert.Contains(content.Account.Roles, "user");
            });
        }
    }
}
