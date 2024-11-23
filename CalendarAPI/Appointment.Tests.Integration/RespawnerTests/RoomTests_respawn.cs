using Appointment.Tests.Integration.Controllers;
using Respawn;

namespace Appointment.Tests.Integration.RespawnerTests;
[TestFixture]
[MemoryDiagnoser]
[SimpleJob(RunStrategy.Throughput)]
public class RoomTests_respawn : RoomTests
{
    private Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactory<Program> _factory;
    private readonly IDatabaseRestoreService _databaseRestoreService = new DatabaseRestoreService();
    private string _connectionString;
    private Respawner _respawner;
    [OneTimeSetUp]
    [GlobalSetup]
    public void OneTimeSetup()
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
        _respawner = Respawner.CreateAsync(_connectionString, new RespawnerOptions
        {
            TablesToIgnore =
            [
                "__EFMigrationsHistory",
            ],
        }).Result;
    }
    [GlobalCleanup]
    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _respawner.ResetAsync(_connectionString).Wait();
        _factory.Dispose();
    }
    [SetUp]
    [IterationSetup]
    public void SetUpAsync()
    {
        _respawner.ResetAsync(_connectionString).Wait();

    }
    [Test]
    [Ignore("benchmark")]
    public async Task GetAllRooms_ShouldReturnCorrect_WithRespawn()
    {
        // Arrange
        var client = _factory.CreateClient();
        using var scope = _factory.Services.CreateScope();
        var scopeService = scope.ServiceProvider;
        using var dbContext = scopeService.GetRequiredService<CalendarContext>();
        dbContext.Database.EnsureCreated();
        dbContext.Rooms.Add(new Room("room 101", rooms: null));
        dbContext.Rooms.Add(new Room("room 102", null));
        dbContext.Rooms.Add(new Room("room 103", null));
        await dbContext.SaveChangesAsync();
        // Act
        var response = await client.GetAsync(HttpHelper.Urls.GetAllRooms);
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<GetAllItemsWithCountDto<GetRoomDto>>();
        // Assert
        result.Should().NotBeNull();
        result!.TotalCount.Should().Be(3);
        result.Items[0].Name.Should().Be("room 101");
        result.Items[1].Name.Should().Be("room 102");
        result.Items[2].Name.Should().Be("room 103");
    }
}