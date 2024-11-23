using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System.Net.Http;
using static System.Net.WebRequestMethods;

namespace Appointment.Tests.Integration.DropTableTests;

[TestFixture]
public class RoomTests
{
    private Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactory<Program> _factory;
    private string _connectionString;
    
    [Test]
    [Ignore("bechmark")]
    public async Task GetAllRooms_ShouldReturnCorrect_WithDropTable()
    {
        //var testServer = new TestServer(new WebHostBuilder().UseConfiguration(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build())
        //                                                    .UseStartup<TestStartup>());
        //var httpClient = testServer.CreateClient();
        //using var dbContext = testServer.Services.GetService<CalendarContext>();
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
        //httpClient.BaseAddress = new Uri("https://localhost:44344/");
        var response = await client.GetAsync(HttpHelper.Urls.GetAllRooms);
        //response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<GetAllItemsWithCountDto<GetRoomDto>>();
        // Assert
        result.TotalCount.Should().Be(3);
        result.Items[0].Name.Should().Be("room 101");
        result.Items[1].Name.Should().Be("room 102");
        result.Items[2].Name.Should().Be("room 103");
        dbContext.Database.ExecuteSqlRaw(
@"ALTER TABLE Visit DROP CONSTRAINT FK_Visit_Room_RoomId
TRUNCATE TABLE Room;
ALTER TABLE Visit ADD CONSTRAINT FK_Visit_Room_RoomId FOREIGN KEY(ID) REFERENCES Visit (ID)");
    }
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
        // make sure the database is created and clean
        var client = _factory.CreateClient();
        using var scope = _factory.Services.CreateScope();
        var scopeService = scope.ServiceProvider;
        using var dbContext = scopeService.GetRequiredService<CalendarContext>();
        dbContext.Database.EnsureCreated();
        dbContext.Database.ExecuteSqlRaw(
@"ALTER TABLE Visit DROP CONSTRAINT FK_Visit_Room_RoomId
TRUNCATE TABLE Room;
ALTER TABLE Visit ADD CONSTRAINT FK_Visit_Room_RoomId FOREIGN KEY(ID) REFERENCES Visit (ID)");
    }
    [OneTimeTearDown]
    [GlobalCleanup]
    public void OneTimeTearDown() { _factory.Dispose(); }
}
