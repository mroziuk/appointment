using Appointment.Domain.DTO.Room;
using Appointment.Domain.Entities;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using Xunit;
namespace Appointment.Tests.Integration_.Controllers
{
    public class RoomTest(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
    {
        [Fact]
        public async Task AddRoom_ReturnsCorrectly()
        {
            await Clean();
            // Arrange
            var NAME = "name 123";
            var content = new StringContent(JsonConvert.SerializeObject(new AddRoomDto() { Name = NAME }), Encoding.UTF8, "application/json");
            // Act
            var response = await _httpClient.PostAsync($"/api/rooms", content);
            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var room = _uow.Rooms.NotDeleted().WithExactName("name 123");
            Assert.NotNull(room);
        }
        [Fact]
        public async Task DeleteRoom_ReturnsCorrectly_WhenEntityExists()
        {
            await Clean();
            // Arrange
            Room room = new Room("name 123", null!);
            _uow.Rooms.Add(room);
            await _uow.SaveChangesAsync();
            // Act
            var response = await _httpClient.DeleteAsync($"/api/rooms/{room.Id}");
            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            var roomDeleted = _uow.Rooms.NotDeleted().WithExactName("name 123");
            Assert.Empty(roomDeleted);
        }

        [Fact]
        public async Task DeleteRoom_ReturnsNotFound_WhenEntityDoesNotExist()
        {
            await Clean();
            // Arrange
            var roomId = 123;
            // Act
            var response = await _httpClient.DeleteAsync($"/api/rooms/{roomId}");
            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.Empty(_uow.Rooms.NotDeleted().WithExactName("name 123"));
        }
        #region setup
        public async Task Clean()
        {
            _uow.Rooms.RemoveRange(_uow.Rooms);
            await _uow.SaveChangesAsync();
        }
        #endregion
    }
}
