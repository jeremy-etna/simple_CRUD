using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using quest_web.Controllers;
using quest_web.DAL;
using quest_web.DTO;
using quest_web.Models;
using quest_web.Utils;
using Xunit;

namespace quest_web.Tests
{
    public class UserControllerTests : IDisposable
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

        private User _adminUser;
        private User _regularUser;

        public UserControllerTests()
        {
            Initialize();
        }

        private void Initialize()
        {
            // Initialization code
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
                Role = UserRole.ROLE_ADMIN,
            };

            _regularUser = new User
            {
                ID = 2,
                Username = _regularUserUsername,
                Password = _regularUserPassword,
                Role = UserRole.ROLE_ADMIN,
            };

            AuthenticateClient(_authenticatedAdminClient, _adminUser).Wait();
            AuthenticateClient(_authenticatedRegularClient, _regularUser).Wait();
        }

        public void Dispose()
        {
            // Cleanup code
            Trace.Listeners.Remove(_traceListener);

            _authenticatedAdminClient.Dispose();
            _authenticatedRegularClient.Dispose();
            _anonymousClient.Dispose();
            _authenticationController.Dispose();
        }

        private async Task<bool> AuthenticateClient(HttpClient client, User usrInfos)
        {
            var loginString = JsonConvert.SerializeObject(usrInfos);

            var result = await client.PostAsync("/authenticate", new StringContent(loginString, Encoding.UTF8, "application/json"));

            if (!result.IsSuccessStatusCode)
            {
                Console.WriteLine("Test initialization failed: cannot login");
                return false;
            }

            var responseDataString = await result.Content.ReadAsStringAsync();
            var responseJson = JsonConvert.DeserializeObject<AuthenticationResponse>(responseDataString);

            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + responseJson.token);

            return true;
        }

        [Fact]
        public async Task GetUserList_OK_When_NotAnonymous()
        {
            var result = await _authenticatedRegularClient.GetAsync("/user");
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public async Task GetUserList_Unauthorized_When_Anonymous()
        {
            var result = await _anonymousClient.GetAsync("/user");
            Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
        }

        [Fact]
        public async Task GetUserByID_OK_When_UsingSelfID()
        {
            var result = await _authenticatedRegularClient.GetAsync($"/user/{_regularUser.ID}");
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public async Task GetUserByID_OK_When_Admin()
        {
            var result = await _authenticatedAdminClient.GetAsync($"/user/{_regularUser.ID}");
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public async Task GetUserByID_Unauthorized_When_Anonymous()
        {
            var result = await _anonymousClient.GetAsync($"/user/{_regularUser.ID}");
            Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
        }

        [Fact]
        public async Task ChangeUsername_OK_When_UsingSelfID()
        {
            var putData = JsonConvert.SerializeObject(new User
            {
                Username = "New_Username"
            });

            var result = await _authenticatedRegularClient.PutAsync($"/user/{_regularUser.ID}", new StringContent(putData, Encoding.UTF8, "application/json"));
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public async Task ChangeUsername_OK_When_UsingSelfID_AndWhen_IsAdmin()
        {
            var putData = JsonConvert.SerializeObject(new User
            {
                Username = "New_Username"
            });

            var result = await _authenticatedAdminClient.PutAsync($"/user/{_regularUser.ID}", new StringContent(putData, Encoding.UTF8, "application/json"));
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public async Task ChangeUsername_Unauthorized_When_Anonymous()
        {
            var putData = JsonConvert.SerializeObject(new User
            {
                Username = "New_Username"
            });

            var result = await _anonymousClient.PutAsync($"/user/{_regularUser.ID}", new StringContent(putData, Encoding.UTF8, "application/json"));
            Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
        }

        [Fact]
        public async Task DeleteUser_OK_When_UsingSelfID()
        {
            var result = await _authenticatedRegularClient.DeleteAsync($"/user/{_regularUser.ID}");
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public async Task DeleteUser_OK_When_UsingSelfID_AndWhen_IsAdmin()
        {
            var result = await _authenticatedAdminClient.DeleteAsync("/user/5");
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public async Task DeleteUser_Unauthorized_When_Anonymous()
        {
            var result = await _anonymousClient.DeleteAsync("/user/4");
            Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
        }

        [Fact]
        public async Task DeleteUser_BadRequest_When_UserIDDoesntExist()
        {
            var result = await _authenticatedAdminClient.DeleteAsync("/user/99999");
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }
    }
}
