using Appointment.Domain.DTO.User;
using Appointment.Tests.Unit.ObjectMothers;
using Respawn;

namespace Appointment.Tests.Integration.DropTableTests
{
    [TestFixture]
    [SimpleJob(RunStrategy.Throughput)]
    public class UserTests //: IClassFixture<TestingWebAppFactory<Program>>
    {
        private Respawner _respawner;
        //private TestingWebAppFactory<Program> _factory;
        private Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactory<Program> _factory;
        private string _connectionString;
        readonly IDatabaseRestoreService _databaseRestoreService = new DatabaseRestoreService();
        [OneTimeSetUp]
        public async Task OneTimeSetup()
        {
            var _builder = WebApplication.CreateBuilder();
            _connectionString = _builder.Configuration.GetConnectionString("sqlserver");
            _factory = new TestingWebAppFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.RemoveAll(typeof(DbContextOptions<CalendarContext>));
                    services.AddDbContext<CalendarContext>(options =>
                    {
                        options.UseSqlServer(_connectionString);
                    });
                });
            });
            _respawner = await Respawner.CreateAsync(_connectionString, new RespawnerOptions
            {
                TablesToIgnore =
                [
                    "__EFMigrationsHistory",
                ],
            });
        }
        [OneTimeTearDown]
        public void OneTimeTearDown() { _factory.Dispose(); }

        [SetUp]
        public void SetUpAsync()
        {
            _respawner.ResetAsync(_connectionString).Wait();
        }
        [Test]
        [Ignore("bechmark")]
        public async Task GetAllUsers_ShouldReturnCorrect()
        {
            // Arrange
            using (var scope = _factory.Services.CreateScope())
            {
                var scopeService = scope.ServiceProvider;
                var dbContext = scopeService.GetRequiredService<CalendarContext>();

                dbContext.Database.EnsureCreated();
                dbContext.Users.Add(EntityFactory.UserFactory.CreateUser("login", "email@email.com", "John", "Krasinsky", "111333222"));
                dbContext.SaveChanges();
            }
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(HttpHelper.Urls.ApiGetAllUsers);
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<GetAllItemsWithCountDto<UserInfoDto>>();
            // Assert

            result!.TotalCount.Should().Be(1);
            result.Page.Should().Be(1);
            result.PageSize.Should().Be(10);
            result.Items[0].FirstName.Should().Be("John");
            result.Items[0].LastName.Should().Be("Krasinsky");
        }
        [Test]
        [Ignore("bechmark")]
        public async Task GetUser_ShouldReturnNotFound_WhenNoUser()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopeService = scope.ServiceProvider;
                var dbContext = scopeService.GetRequiredService<CalendarContext>();

                dbContext.Database.EnsureCreated();
                dbContext.Users.Add(EntityFactory.UserFactory.CreateUser("login", "email@email.com", "John", "Krasinsky", "111333222"));
                dbContext.SaveChanges();
            }
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(string.Format(HttpHelper.Urls.GetUser, 2));
            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }
        [Test]
        [Ignore("bechmark")]
        public async Task GetUser_ShouldReturnCorrectly()
        {
            var user = EntityFactory.UserFactory.CreateUser("login", "email@email.com", "John", "Krasinsky", "111333222");
            using (var scope = _factory.Services.CreateScope())
            {
                var scopeService = scope.ServiceProvider;
                var dbContext = scopeService.GetRequiredService<CalendarContext>();
                dbContext.Database.EnsureCreated();
                dbContext.Users.Add(user);
                dbContext.SaveChanges();
            }
            var client = _factory.CreateClient();
            // Act
            var response = await client.GetAsync(string.Format(HttpHelper.Urls.GetUser, user.Id));
            var result = await response.Content.ReadFromJsonAsync<UserInfoDto>();
            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            result.FirstName.Should().Be("John");
            result.LastName.Should().Be("Krasinsky");
        }
    }
}
