using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using quest_web.DAL;
using quest_web.Models;

namespace quest_web.Tests
{
    public class QuestWebWebApplicationFactory : WebApplicationFactory<Startup>
    {
        public APIDbContext _context;

        protected void AddTestData(APIDbContext context)
        {
            // Add admin user
            context.User.Add(new User
            {
                Username = "admin",
                Password = BCrypt.Net.BCrypt.HashPassword("admin_password"),
                Role = UserRole.ROLE_ADMIN,
                Creation_Date = DateTime.Now,
                Updated_Date = DateTime.Now,
            });

            context.Address.Add(new Address
            {
                street = "Admin Road",
                postalCode = "Admin Postcode",
                city = "Admin City",
                country = "Admin Country",
                creationDate = DateTime.Now,
                updatedDate = DateTime.Now,
                User = 1
            });

            // Add test users
            for (int i = 2; i <= 12; i++)
            {
                context.User.Add(new User
                {
                    Username = $"user{i}",
                    Password = BCrypt.Net.BCrypt.HashPassword("user_password"),
                    Role = UserRole.ROLE_USER,
                    Creation_Date = DateTime.Now,
                    Updated_Date = DateTime.Now,
                });

                context.Address.Add(new Address
                {
                    street = $"User{i} Road",
                    postalCode = "User Postcode",
                    city = "User City",
                    country = "User Country",
                    creationDate = DateTime.Now,
                    updatedDate = DateTime.Now,
                    User = i
                });
            }

            context.SaveChanges();
            Console.WriteLine(context.Address);
        }

        private void CleanDatabaseContextFromService(IServiceCollection services)
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<APIDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Clean up previous DbContext configuration
                CleanDatabaseContextFromService(services);

                // Setup in-memory database for testing
                var serviceProvider = new ServiceCollection()
                    .AddEntityFrameworkInMemoryDatabase()
                    .BuildServiceProvider();

                services.AddDbContext<APIDbContext>(options =>
                {
                    options.UseInMemoryDatabase("Tests");
                    options.UseInternalServiceProvider(serviceProvider);
                });

                // Create service scope to configure the DbContext
                using var scope = services.BuildServiceProvider().CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<APIDbContext>();

                context.Database.EnsureCreated();
                AddTestData(context);
                _context = context;
                Console.WriteLine("CONFIGURE WEB HOST CALLED");
            });
        }
    }
}
