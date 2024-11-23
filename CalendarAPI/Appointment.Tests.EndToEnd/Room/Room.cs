using Appointment.Domain.DTO;
using Appointment.Domain.DTO.Room;
using Appointment.Domain.Entities;
using Dapper;
using EmptyFiles;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Shouldly;
using System.Data;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using static Appointment.Api.Extensions.CreateInitDataExtension;

namespace Appointment.Tests.EndToEnd.Room
{
    [TestFixture]
    public class Room : TestBaseE2E
    {
        [SetUp]
        public async Task SetUpAsync()
        {

        }
        [Test]
        public async Task Will_return_all_rooms()
        {
            var room = Guid.NewGuid().ToString();
            string deleteRoomQuery = "delete from [dbo].[Room] where [dbo].[Room].Name like '" + room + "'";

            var token = await Login("admin@admin.admin", "Admin123", "superadmin");
            var roomRequest = new HttpRequestMessage(HttpMethod.Get, $"{Api}/rooms/all?name={room}");
            roomRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

            using IDbConnection db = new SqlConnection(_connectionString);
            await db.QueryAsync(deleteRoomQuery);

            _ = await AddRoom(room, token.AccessToken, HttpStatusCode.Created);
            var roomResponse = await HttpClient.SendAsync(roomRequest);
            roomResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
            var content = await roomResponse.Content.ReadAsStringAsync();
            content.ShouldNotBeNullOrEmpty();

            var result = JsonConvert.DeserializeObject<GetAllItemsWithCountDto<GetRoomDto>>(content);
            result.TotalCount.ShouldBeGreaterThan(0);
            result.Items.ShouldContain(r => r.Name == room);

            await db.QueryAsync(deleteRoomQuery);
        }
        [Test]
        public async Task Will_add_room_as_admin()

        {
            // create new room and ensure it is not in database
            string room = Guid.NewGuid().ToString();
            string deleteCreatedRoomQuery = "delete from [dbo].[Room] where [dbo].[Room].Name like '" + room + "'";
            using IDbConnection db = new SqlConnection(_connectionString);
            await db.QueryAsync(deleteCreatedRoomQuery);

            // login as admin and add room
            var token = await Login(AdminEmail, AdminPassword, "admin");
            var response = await AddRoom(room, token.AccessToken, HttpStatusCode.Created);

            // get room and check if it is in database
            var getRoomRequest = new HttpRequestMessage(HttpMethod.Get, response.Headers.Location);
            getRoomRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
            var getRoomResponse = await HttpClient.SendAsync(getRoomRequest);
            getRoomResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
            var content = await getRoomResponse.Content.ReadAsStringAsync();
            content.ShouldNotBeNullOrEmpty();
            var result = JsonConvert.DeserializeObject<GetRoomDto>(content);
            result!.Name.ShouldBe(room);

            // delete room
            await db.QueryAsync(deleteCreatedRoomQuery);
        }
        [Test]
        public async Task Will_not_add_room_as_user()
        {
            string room = Guid.NewGuid().ToString();

            var token = await Login(UserEmail, UserPassword, "user");

            _ = await AddRoom(room, token.AccessToken, HttpStatusCode.Forbidden);

        }
        [Test]
        public async Task Will_not_add_room_without_token()
        {
            string room = Guid.NewGuid().ToString();
            _ = await AddRoom(room, "", HttpStatusCode.Unauthorized);
        }
        [Test]
        public async Task Will_delete_room_as_admin()
        {
            // create room and ensure it is not in database
            var room = Guid.NewGuid().ToString();
            string deleteRoomCategoryQuery = "delete from [dbo].[Room] where [dbo].[Room].Name like '" + room + "'";
            var token = await Login(AdminEmail, AdminPassword, "admin");
            using IDbConnection db = new SqlConnection(_connectionString);
            await db.QueryAsync(deleteRoomCategoryQuery);

            // add room
            var response = await AddRoom(room, token.AccessToken, HttpStatusCode.Created);
            // get room
            var getRoomRequest = new HttpRequestMessage(HttpMethod.Get, response.Headers.Location);
            getRoomRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
            var getRoomResponse = await HttpClient.SendAsync(getRoomRequest);
            getRoomResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
            var content = await getRoomResponse.Content.ReadAsStringAsync();
            content.ShouldNotBeNullOrEmpty();
            var result = JsonConvert.DeserializeObject<GetRoomDto>(content);
            result!.Name.ShouldBe(room);
            // delete room
            var deleteRoomRequest = new HttpRequestMessage(HttpMethod.Delete, response.Headers.Location);
            deleteRoomRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
            var deleteRoomResponse = await HttpClient.SendAsync(deleteRoomRequest);
            deleteRoomResponse.StatusCode.ShouldBe(HttpStatusCode.NoContent);
            // ensure we don't get room from database
            var getRoomRequest2 = new HttpRequestMessage(HttpMethod.Get, response.Headers.Location);
            getRoomRequest2.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
            getRoomResponse = await HttpClient.SendAsync(getRoomRequest2);
            getRoomResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);

            // clear database
            await db.QueryAsync(deleteRoomCategoryQuery);
        }
        [Test]
        public async Task Will_not_delete_room_as_user()
        {
            var room = Guid.NewGuid().ToString();

            var token = await Login(UserEmail, UserPassword, "user");

            _ = await AddRoom(room, token.AccessToken, HttpStatusCode.Forbidden);
        }
        [Test]
        public async Task Will_not_delete_room_without_token()
        {
            var room = Guid.NewGuid().ToString();

            var token = await Login(UserEmail, UserPassword, "user");

            _ = await AddRoom(room, "", HttpStatusCode.Unauthorized);
        }
        [Test]
        public async Task Will_update_room_as_admin()
        {
            // create room and ensure it is not in database
            var room = Guid.NewGuid().ToString();
            var newRoom = Guid.NewGuid().ToString();
            string deleteRoomCategoryQuery = "delete from [dbo].[Room] where [dbo].[Room].Name like '" + room + "'";
            string deleteNewRoomCategoryQuery = "delete from [dbo].[Room] where [dbo].[Room].Name like '" + newRoom + "'";
            var token = await Login(AdminEmail, AdminPassword, "admin");
            using IDbConnection db = new SqlConnection(_connectionString);
            await db.QueryAsync(deleteRoomCategoryQuery);

            // add room
            var response = await AddRoom(room, token.AccessToken, HttpStatusCode.Created);
            // get room
            var getRoomRequest = new HttpRequestMessage(HttpMethod.Get, response.Headers.Location);
            getRoomRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
            var getRoomResponse = await HttpClient.SendAsync(getRoomRequest);
            getRoomResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
            var content = await getRoomResponse.Content.ReadAsStringAsync();
            content.ShouldNotBeNullOrEmpty();
            var result = JsonConvert.DeserializeObject<GetRoomDto>(content);
            result!.Name.ShouldBe(room);

            // update room
            var updateBody = new { Name = newRoom };
            var updateRoomRequest = new HttpRequestMessage(HttpMethod.Patch, response.Headers.Location);
            updateRoomRequest.Content = new StringContent(JsonConvert.SerializeObject(updateBody), Encoding.UTF8, "application/json-patch+json");
            updateRoomRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
            var updateRoomResponse = await this.HttpClient.SendAsync(updateRoomRequest);
            updateRoomResponse.StatusCode.ShouldBe(HttpStatusCode.NoContent);
            // get room
            var getRoomRequest2 = new HttpRequestMessage(HttpMethod.Get, response.Headers.Location);
            getRoomRequest2.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
            var getRoomResponse2 = await HttpClient.SendAsync(getRoomRequest2);
            getRoomResponse2.StatusCode.ShouldBe(HttpStatusCode.OK);
            var content2 = await getRoomResponse2.Content.ReadAsStringAsync();
            content2.ShouldNotBeNullOrEmpty();
            var result2 = JsonConvert.DeserializeObject<GetRoomDto>(content2);
            result2!.Name.ShouldBe(newRoom);

            // delete room
            await db.QueryAsync(deleteRoomCategoryQuery);
            await db.QueryAsync(deleteNewRoomCategoryQuery);
        }
        [Test]
        public async Task Will_not_update_room_as_user()
        {
            var room = Guid.NewGuid().ToString();

            var token = await Login(UserEmail, UserPassword, "user");

            _ = await AddRoom(room, token.AccessToken, HttpStatusCode.Forbidden);
        }
        [Test]
        public async Task Will_not_update_room_without_token()
        {
            var room = Guid.NewGuid().ToString();

            var token = await Login(UserEmail, UserPassword, "user");

            _ = await AddRoom(room, "", HttpStatusCode.Unauthorized);
        }
    }
}
