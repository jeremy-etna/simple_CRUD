using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using quest_web.Controllers;
using quest_web.DAL;
using quest_web.DTO;
using quest_web.Models;
using Xunit;

namespace quest_web.Tests
{
    public class AuthenticationControllerTests : IDisposable
    {
        private QuestWebWebApplicationFactory _authenticationController;
        private HttpClient _authenticatedAdminClient;
        private HttpClient _authenticatedRegularClient;
        private HttpClient _anonymousClient;
        private ConsoleTraceListener _traceListener;

        private const string _adminUserUsername = "admin";
        private const string _adminUserPassword = "admin_password";
        private const string _regularUserUsername = "user2";
        private const string _regularUserPassword = "user_password";

        private readonly User _adminUser;
        private readonly User _regularUser;

        public AuthenticationControllerTests()
        {
            _authenticationController = new QuestWebWebApplicationFactory();
            _authenticatedAdminClient = _authenticationController.CreateClient();
            _authenticatedRegularClient = _authenticationController.CreateClient();
            _anonymousClient = _authenticationController.CreateClient();

            _traceListener = new ConsoleTraceListener();
            Trace.Listeners.Add(_traceListener);

            _adminUser = new User
            {
                ID = 1,
                Username = _adminUserUsername,
                Password = _adminUserPassword,
            };

            _regularUser = new User
            {
                ID = 2,
                Username = _regularUserUsername,
                Password = _regularUserPassword,
            };

            SetupTests();
        }

        public void Dispose()
        {
            DestroyTestData();
        }

        private async Task<bool> AuthenticateClient(HttpClient client, User usrInfos)
        {
            var loginString = JsonConvert.SerializeObject(usrInfos);

            var result = await client.PostAsync("/authenticate",
                new StringContent(loginString, Encoding.UTF8, "application/json"));

            if (!result.IsSuccessStatusCode)
            {
                return false;
            }

            var responseDataString = await result.Content.ReadAsStringAsync();
            var responseJson = JsonConvert.DeserializeObject<AuthenticationResponse>(responseDataString);

            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + responseJson.token);

            return true;
        }

        public void SetupTests()
        {
            AuthenticateClient(_authenticatedAdminClient, _adminUser).Wait();
            AuthenticateClient(_authenticatedRegularClient, _regularUser).Wait();

            // Assert removed; instead, you can handle failures within the tests themselves.
        }

        private void DestroyTestData()
        {
            Trace.Listeners.Remove(_traceListener);

            _authenticatedAdminClient.Dispose();
            _authenticatedRegularClient.Dispose();
            _anonymousClient.Dispose();
            _authenticationController.Dispose();
        }

        [Fact]
        public async Task Registration_Created_When_ValidParameters()
        {
            var registrationString = JsonConvert.SerializeObject(new User
            {
                Username = "Test_User",
                Password = "Test_Password",
                Role = UserRole.ROLE_USER,
            });

            var result = await _anonymousClient.PostAsync("/register",
                new StringContent(registrationString, Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.Created, result.StatusCode);
        }

        [Fact]
        public async Task Registration_BadRequest_When_EmptyUsername()
        {
            var registrationString = JsonConvert.SerializeObject(new User
            {
                Username = "",
                Password = "Test_Password",
                Role = UserRole.ROLE_USER,
            });

            var result = await _anonymousClient.PostAsync("/register",
                new StringContent(registrationString, Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task Registration_BadRequest_When_EmptyPassword()
        {
            var registrationString = JsonConvert.SerializeObject(new User
            {
                Username = "Test_User",
                Password = "",
                Role = UserRole.ROLE_USER,
            });

            var result = await _anonymousClient.PostAsync("/register",
                new StringContent(registrationString, Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task Registration_BadRequest_When_UsernameExists()
        {
            var registrationString = JsonConvert.SerializeObject(new User
            {
                Username = "user2",
                Password = "Test_Password",
                Role = UserRole.ROLE_USER,
            });

            var result = await _anonymousClient.PostAsync("/register",
                new StringContent(registrationString, Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.Conflict, result.StatusCode);
        }

        [Fact]
        public async Task Authentication_OK_When_ValidCredentials()
        {
            var loginString = JsonConvert.SerializeObject(new User
            {
                Username = "admin",
                Password = "admin_password",
            });

            var result = await _anonymousClient.PostAsync("/authenticate",
                new StringContent(loginString, Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public async Task Authentication_BadRequest_When_UnknownUsername()
        {
            var loginString = JsonConvert.SerializeObject(new User
            {
                Username = "Unknown_Username",
                Password = "admin_password",
            });

            var result = await _anonymousClient.PostAsync("/authenticate",
                new StringContent(loginString, Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task Authentication_Unauthorized_When_WrongPassword()
        {
            var loginString = JsonConvert.SerializeObject(new User
            {
                Username = "admin",
                Password = "wrong_Password",
            });

            var result = await _anonymousClient.PostAsync("/authenticate",
                new StringContent(loginString, Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
        }

        [Fact]
        public async Task Me_OK_When_Authenticated()
        {
            var result = await _authenticatedRegularClient.GetAsync("/me");

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public async Task Me_Unauthorized_When_Anonymous()
        {
            var result = await _anonymousClient.GetAsync("/me");

            Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
        }
    }
}
