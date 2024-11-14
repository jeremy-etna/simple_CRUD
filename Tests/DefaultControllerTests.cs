using System.Net;
using System.Text;
using Newtonsoft.Json;
using quest_web.Models;
using Xunit;
using Xunit.Abstractions;

namespace quest_web.Tests
{
    public class DefaultTestContext : IDisposable
    {
        private QuestWebWebApplicationFactory _authenticationController;
        public HttpClient _authenticatedAdminClient;
        public HttpClient _authenticatedRegularClient;
        public HttpClient _anonymousClient;

        public const string _adminUserUsername = "admin";
        public const string _adminUserPassword = "admin_password";

        public const string _regularUserUsername = "user2";
        public const string _regularUserPassword = "user_password";

        public readonly User _adminUser;
        public readonly User _regularUser;

        public DefaultTestContext()
        {
            _authenticationController = new QuestWebWebApplicationFactory();
            _authenticatedAdminClient = _authenticationController.CreateClient();
            _authenticatedRegularClient = _authenticationController.CreateClient();
            _anonymousClient = _authenticationController.CreateClient();

            _adminUser = new User
            {
                ID = 1,
                Username = _adminUserUsername,
                Password = _adminUserPassword,
                Role = UserRole.ROLE_ADMIN,
            };

            _regularUser = new User
            {
                ID = 2,
                Username = _regularUserUsername,
                Password = _regularUserPassword,
                Role = UserRole.ROLE_ADMIN,
            };

            SetupTests();
        }

        private async Task<bool> AuthenticateClient(HttpClient client, User usrInfos)
        {
            var loginString = JsonConvert.SerializeObject(usrInfos);
            var result = await client.PostAsync("/authenticate",
                new StringContent(loginString, Encoding.UTF8, "application/json"));

            if (!result.IsSuccessStatusCode)
                return false;

            var jwtToken = await result.Content.ReadAsStringAsync();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + jwtToken);

            return true;
        }

        public void SetupTests()
        {
            AuthenticateClient(_authenticatedAdminClient, _adminUser).Wait();
            AuthenticateClient(_authenticatedRegularClient, _regularUser).Wait();
        }

        public void Dispose()
        {
            _authenticatedAdminClient.Dispose();
            _authenticatedRegularClient.Dispose();
            _anonymousClient.Dispose();
            _authenticationController.Dispose();
        }
    }

    public class DefaultControllerTests : IClassFixture<DefaultTestContext>
    {
        private DefaultTestContext _context;
        private readonly ITestOutputHelper _output;
        public DefaultControllerTests(DefaultTestContext context)
        {
            _context = context;
        }

        [Fact]
        public async Task GetSuccess_OK()
        {
            var result = await _context._anonymousClient.GetAsync("/testSuccess");
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public async Task GetNotFound_KO()
        {
            var result = await _context._anonymousClient.GetAsync("/testNotFound");
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task GetError_KO()
        {
            var result = await _context._anonymousClient.GetAsync("/testError");
            Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
        }
    }
}
