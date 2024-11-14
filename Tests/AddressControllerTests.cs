using System.Net;
using System.Text;
using Newtonsoft.Json;
using quest_web.DAL;
using quest_web.Models;
using Xunit;
using Xunit.Abstractions;
using quest_web.DTO;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace quest_web.Tests
{
    public class AddressTestContext : IDisposable
    {
        private QuestWebWebApplicationFactory _authenticationController;
        public HttpClient _authenticatedAdminClient;
        public HttpClient _authenticatedRegularClient;
        public HttpClient _authenticatedRegularClient2;
        public HttpClient _anonymousClient;

        public const string _adminUserUsername = "admin";
        public const string _adminUserPassword = "admin_password";

        public const string _regularUserUsername = "user2";
        public const string _regularUserPassword = "user_password";

        public const string _regularUserUsername2 = "user3";
        public const string _regularUserPassword2 = "user_password";

        public readonly User _adminUser;
        public readonly User _regularUser;
        public readonly User _regularUser2;

        public AddressTestContext()
        {
            _authenticationController = new QuestWebWebApplicationFactory();
            _authenticatedAdminClient = _authenticationController.CreateClient();
            _authenticatedRegularClient = _authenticationController.CreateClient();
            _authenticatedRegularClient2 = _authenticationController.CreateClient();
            _anonymousClient = _authenticationController.CreateClient();

            Console.WriteLine(_authenticationController._context.Address.ToString());

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

            _regularUser2 = new User
            {
                ID = 3,
                Username = _regularUserUsername2,
                Password = _regularUserPassword2,
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

            var responseDataString = await result.Content.ReadAsStringAsync();
            var responseJson = JsonConvert.DeserializeObject<AuthenticationResponse>(responseDataString);

            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + responseJson.token);

            return true;
        }

        public void SetupTests()
        {
            AuthenticateClient(_authenticatedAdminClient, _adminUser).Wait();
            AuthenticateClient(_authenticatedRegularClient, _regularUser).Wait();
            AuthenticateClient(_authenticatedRegularClient2, _regularUser2).Wait();
        }

        public void Dispose()
        {
            _authenticatedAdminClient.Dispose();
            _authenticatedRegularClient.Dispose();
            _authenticatedRegularClient2.Dispose();
            _anonymousClient.Dispose();
            _authenticationController.Dispose();
        }
    }

    public class AddressControllerTests : IClassFixture<AddressTestContext>
    {
        private AddressTestContext _context;
        private readonly ITestOutputHelper _output;
        public AddressControllerTests(AddressTestContext context, ITestOutputHelper output)
        {
            _context = context;
            _output = output;
        }

        [Fact]
        public async Task GetAddress_OK_When_Authenticated()
        {
            var result = await _context._authenticatedRegularClient.GetAsync("/address");

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public async Task GetAddress_Unauthorized_When_Anonymous()
        {
            var result = await _context._anonymousClient.GetAsync("/address");
            Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
        }

        [Fact]
        public async Task GetAddressByID_Unauthorized_When_Anonymous()
        {
            var result = await _context._anonymousClient.GetAsync("/address");
            Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
        }

        [Fact]
        public async Task GetAddressByID_Ok_When_Authenticated()
        {
            var result = await _context._authenticatedRegularClient.GetAsync("/address");
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public async Task CreateAddress_Ok_When_Authenticated_AndWhen_ValidAddress()
        {
            var newAddress = JsonConvert.SerializeObject(new Address
            {
                city = "Paris",
                country = "France",
                postalCode = "94000",
                street = "Champs Elysees"
            });

            var result = await _context._authenticatedRegularClient.PostAsync("/address",
                new StringContent(newAddress, Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.Created, result.StatusCode);
        }

        [Fact]
        public async Task CreateAddress_Unauthorized_When_Anonymous()
        {
            var newAddress = JsonConvert.SerializeObject(new Address
            {
                city = "Paris",
                country = "France",
                postalCode = "94000",
                street = "Champs Elysees"
            });

            var result = await _context._anonymousClient.PostAsync("/address",
                new StringContent(newAddress, Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
        }

        [Fact]
        public async Task CreateAddress_BadRequest_When_Authenticated_AndWhen_InvalidAddress()
        {
            var newAddress = JsonConvert.SerializeObject(new Address
            {
                city = "",
                country = "France",
                postalCode = "94000",
                street = "Champs Elysees"
            });

            var result = await _context._authenticatedRegularClient.PostAsync("/address",
                new StringContent(newAddress, Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task ChangeAddress_Ok_When_Authenticated_AndWhen_ValidAddress()
        {
            var newAddress = JsonConvert.SerializeObject(new Address
            {
                city = "Paris",
                country = "France",
                postalCode = "94000",
                street = "Champs Elysees"
            });

            var result = await _context._authenticatedRegularClient.PutAsync("/address/2",
                new StringContent(newAddress, Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public async Task ChangeAddress_Unauthorized_When_Anonymous()
        {
            var newAddress = JsonConvert.SerializeObject(new Address
            {
                city = "Paris",
                country = "France",
                postalCode = "94000",
                street = "Champs Elysees"
            });

            var result = await _context._anonymousClient.PutAsync("/address/2",
                new StringContent(newAddress, Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
        }

        [Fact]
        public async Task ChangeAddress_OK_When_Authenticated_AndWhen_EmptyCityAddress()
        {
            var newAddress = JsonConvert.SerializeObject(new Address
            {
                city = "",
                country = "France",
                postalCode = "94000",
                street = "Champs Elysees"
            });

            var result = await _context._authenticatedRegularClient.PutAsync("/address/2",
                new StringContent(newAddress, Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public async Task DeleteAddress_OK_When_Authenticated()
        {
            var result = await _context._authenticatedRegularClient2.DeleteAsync("/address/3");
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public async Task DeleteAddress_Unauthorized_When_Anonymous()
        {
            var result = await _context._anonymousClient.DeleteAsync("/address/1");
            Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
        }

        [Fact]
        public async Task DeleteAddress_BadRequest_When_BadAddressID()
        {
            var result = await _context._authenticatedRegularClient.DeleteAsync("/address/9999");
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }
    }
}
