using Appointment.Domain.Entities.Identity;
using Newtonsoft.Json;
using System.Net;
using Shouldly;
using Appointment.Domain.DTO.User;
using Microsoft.Data.SqlClient;
using Dapper;
using System.Net.Http.Headers;
using Appointment.Domain.DTO.Visit;
using static Appointment.Api.Extensions.CreateInitDataExtension;

namespace Appointment.Tests.EndToEnd.Visit
{
    [TestFixture]
    public class Visit : TestBaseE2E
    {
        [Test]
        public async Task Will_Add_Visit_As_Admin()
        {   
            //await ResetDatabase();
            // const
            var visitDate = new DateTime(2024, 01, 01, 12, 30, 00);
            var visitDuration = TimeSpan.FromHours(1);

            string room = Guid.NewGuid().ToString();
            string deleteCreatedRoomQuery = $"delete from [dbo].[Room] where [dbo].[Room].Name like '{room}'";
            string patientName = Guid.NewGuid().ToString();
            string deleteCreatedPatientQuery = $"delete from [dbo].[Patient] where [dbo].[Patient].FirstName like '{patientName}'";
            //clean up availability table before test
            string deleteAvailabilityQuery = $"delete from [dbo].[Availabillity]";
            using var db = new SqlConnection(_connectionString);
            await db.QueryAsync(deleteAvailabilityQuery);

            // logowanie i pobranie id
            var token = await Login(AdminEmail, AdminPassword, Role.Admin);
            var meResponse = await GetUserMe(token.AccessToken, HttpStatusCode.OK);
            var content = await meResponse.Content.ReadAsStringAsync();
            content.ShouldNotBeNullOrEmpty();
            UserInfoDto result = JsonConvert.DeserializeObject<UserInfoDto>(content);
            var userId = result.Id;

            // dodanie pokoju
            var roomResponse = await AddRoom(room, token.AccessToken, HttpStatusCode.Created);
            var roomId = roomResponse.Headers.Location.AbsolutePath.Split("/").Last();
            int.TryParse(roomId, out int roomIdInt);

            // dodanie dyspozycji
            var availabilityResponse =
            await AddAvailability(userId,
                DayOfWeek.Monday,
                TimeOnly.Parse("08:00:00"),
                TimeOnly.Parse("17:00:00"),
                token.AccessToken,
                HttpStatusCode.Created);
            var availabilityId = availabilityResponse.Headers.Location.AbsolutePath.Split("/").Last();
            int.TryParse(availabilityId, out int availabilityIdInt);

            // dodanie pacjenta
            var patientResponse = await AddPatient(patientName, "Mach", "666222888", token.AccessToken, HttpStatusCode.Created);
            var patientId = patientResponse.Headers.Location.AbsolutePath.Split("/").Last();
            int.TryParse(patientId, out int patientIdInt);

            // dodanie wizyty
            var visitResponse = await AddVisit(userId, patientIdInt,roomIdInt, visitDate, visitDuration, token.AccessToken, HttpStatusCode.Created);
            // sprawdzenie czy wizyta istnieje
            var visitRequest = new HttpRequestMessage(HttpMethod.Get, visitResponse.Headers.Location);
            visitRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
            var response = await this.HttpClient.SendAsync(visitRequest);

            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            var visitContent = await response.Content.ReadAsStringAsync();
            visitContent.ShouldNotBeNullOrEmpty();
            var visitDto = JsonConvert.DeserializeObject<GetVisitDto>(visitContent);
            visitDto.StartDate.ShouldBe(visitDate);
            visitDto.EndDate.ShouldBe(visitDate.Add(visitDuration));

            //clean up database after test
            await db.QueryAsync(deleteCreatedRoomQuery);
            await db.QueryAsync(deleteCreatedPatientQuery);
            await db.QueryAsync($"delete from [dbo].[Availabillity] where [dbo].[Availabillity].Id = {availabilityIdInt}");
        }
        [Test]

        public async Task User_will_create_visit_for_himself()
        {
            //await ResetDatabase();
            string roomName = Guid.NewGuid().ToString();
            string userName1 = "user1@user.user";
            string patientName = Guid.NewGuid().ToString();
            // logowanie i pobranie id
            var adminToken = await Login(AdminEmail, AdminPassword, Role.Admin);
            // dodanie pacjenta
            var patientResponse = await AddPatient("Jan", patientName, "666222888", adminToken.AccessToken, HttpStatusCode.Created);
            int.TryParse(patientResponse.Headers.Location.AbsolutePath.Split("/").Last(), out var patientId);
            // dodanie pokoju
            var roomResponse = await AddRoom(roomName, adminToken.AccessToken, HttpStatusCode.Created);
            int.TryParse(roomResponse.Headers.Location.AbsolutePath.Split("/").Last(), out int roomIdInt);
            // dodanie użytkownika
            var user1 = await Register(userName1, UserPassword, Role.User, userName1, userName1);
            int.TryParse(user1.Headers.Location.AbsolutePath.Split("/").Last(), out int user1Id);
            await Activate(user1Id, adminToken.AccessToken, HttpStatusCode.OK);
            // logowanie i pobranie id
            var userToken = await Login(userName1, UserPassword, Role.User);
            var meResponse = await GetUserMe(userToken.AccessToken, HttpStatusCode.OK);
            var content = await meResponse.Content.ReadAsStringAsync();
            content.ShouldNotBeNullOrEmpty();
            UserInfoDto result = JsonConvert.DeserializeObject<UserInfoDto>(content);
            var userId1 = result.Id;

            // dodanie dyspozycji dla użytkownika
            var availabilityResponse = await AddAvailability(
                userId1,
                DayOfWeek.Monday,
                TimeOnly.Parse("08:00:00"),
                TimeOnly.Parse("17:00:00"),
                userToken.AccessToken,
                HttpStatusCode.Created);

            var visitResponse = await AddVisit(
                userId1,
                patientId,
                roomIdInt,
                new DateTime(2024, 01, 01, 12, 30, 00),
                TimeSpan.FromHours(1),
                userToken.AccessToken,
                HttpStatusCode.Created);

            var db = new SqlConnection(_connectionString);
            await db.QueryAsync($"delete from [dbo].[Room] where [dbo].[Room].Name like '{roomName}'");
            await db.QueryAsync($"delete from [dbo].[Patient] where [dbo].[Patient].FirstName like '{patientName}'");
        }


        [Test]
        public async Task Will_not_add_visit_as_user_for_another_user()
        {
            //await ResetDatabase();
            string roomName = Guid.NewGuid().ToString();
            string userName1 = "user1@user.user";
            string username2 = "user2@user.user";
            string patientName = Guid.NewGuid().ToString();
            // logowanie i pobranie id
            var adminToken = await Login(AdminEmail, AdminPassword, Role.Admin);
            // dodanie pacjenta
            var patientResponse = await AddPatient("Jan", patientName, "666222888", adminToken.AccessToken, HttpStatusCode.Created);
            int.TryParse(patientResponse.Headers.Location.AbsolutePath.Split("/").Last(), out var patientId);
            // dodanie pokoju
            var roomResponse = await AddRoom(roomName, adminToken.AccessToken, HttpStatusCode.Created);
            int.TryParse(roomResponse.Headers.Location.AbsolutePath.Split("/").Last(), out int roomIdInt);
            // dodanie użytkowników
            var user1 = await Register(userName1, UserPassword, Role.User, userName1, userName1);
            var user2 = await Register(username2, UserPassword, Role.User, username2, username2);
            int.TryParse(user1.Headers.Location.AbsolutePath.Split("/").Last(), out int user1Id);
            int.TryParse(user2.Headers.Location.AbsolutePath.Split("/").Last(), out int user2Id);
            await Activate(user1Id, adminToken.AccessToken, HttpStatusCode.OK);
            await Activate(user2Id, adminToken.AccessToken, HttpStatusCode.OK);
            // logowanie i pobranie id
            var token1 = await Login(userName1, UserPassword, Role.User);
            var meResponse = await GetUserMe(token1.AccessToken, HttpStatusCode.OK);
            var content = await meResponse.Content.ReadAsStringAsync();
            content.ShouldNotBeNullOrEmpty();
            UserInfoDto result = JsonConvert.DeserializeObject<UserInfoDto>(content);
            var userId1 = result.Id;

            // dodanie dyspozycji dla użytkownika 1
            var availabilityResponse = await AddAvailability(
                userId1,
                DayOfWeek.Monday,
                TimeOnly.Parse("08:00:00"),
                TimeOnly.Parse("17:00:00"),
                token1.AccessToken, 
                HttpStatusCode.Created);

            var token2 = await Login(username2, UserPassword, Role.User);
            // próba dodania wizyty dla użytkownika 1 przez użytkownika 2
            var visitResponse = await AddVisit(
                userId1,
                patientId,
                roomIdInt,
                new DateTime(2024, 01, 01, 12, 30, 00),
                TimeSpan.FromHours(1),
                token2.AccessToken,
                HttpStatusCode.Forbidden);
        }
        [Test]
        public async Task Will_not_create_visit_without_token()
        {
            _ = await AddVisit(1, 1, 1, new DateTime(2024, 01, 01, 12, 30, 00), TimeSpan.FromHours(1), null, HttpStatusCode.Unauthorized);
        }





        [Test]
        public async Task Will_not_delete_visit_without_token()
        {
            _ = await DeleteVisit(1, null, HttpStatusCode.Unauthorized);
        }
    }
}
