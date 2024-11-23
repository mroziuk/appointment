using Appointment.Domain.DTO;
using Appointment.Domain.DTO.User;
using Azure;
using Dapper;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Shouldly;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json.Nodes;
using static Appointment.Api.Extensions.CreateInitDataExtension;
using static Appointment.Common.Utils.HttpHelper;
namespace Appointment.Tests.EndToEnd.User
{
    [TestFixture]
    public class User : TestBaseE2E
    {
        // admin
        [Test]
        public async Task Will_add_user_as_admin()
        {
            string email = "adam.kowalski@gmail.com";
            string password = "Password1";
            string firstName = "Adam";
            string lastName = "Kowalski";
            string role = "user";
            // ensure that user is not in database
            using var db = new SqlConnection(_connectionString);
            string deleteUserQuery = $"delete from [dbo].[User] where [dbo].[User].Email like '{email}'";
            await db.QueryAsync(deleteUserQuery);
            // login as admin and add user
            var token = await Login(AdminEmail, AdminPassword, "admin");
            var response = await AddUser(email, password, firstName, lastName, role, token.AccessToken, HttpStatusCode.Created);
            // get user and check if it is in database
            var getUserRequest = new HttpRequestMessage(HttpMethod.Get, response.Headers.Location);
            getUserRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
            var getUserResponse = await HttpClient.SendAsync(getUserRequest);

            getUserResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
            var content = await getUserResponse.Content.ReadAsStringAsync();
            content.ShouldNotBeNullOrEmpty();
            var result = JsonConvert.DeserializeObject<UserInfoDto>(content);
            result.Email.ShouldBe(email);
            result.FirstName.ShouldBe(firstName);
            result.LastName.ShouldBe(lastName);
            result.Role.ShouldBe(role);
            // clear database
            await db.QueryAsync(deleteUserQuery);
        }
        [Test]
        public async Task Will_delete_user_as_admin()
        {
            string email = "adam.kowalski@gmail.com";
            string password = "Password1";
            string firstName = "Adam";
            string lastName = "Kowalski";
            string role = "user";
            // ensure that user is not in database
            using var db = new SqlConnection(_connectionString);
            string deleteUserQuery = $"delete from [dbo].[User] where [dbo].[User].Email like '{email}'";
            await db.QueryAsync(deleteUserQuery);
            // login as admin and add user
            var token = await Login(AdminEmail, AdminPassword, "admin");
            var response = await AddUser(email, password, firstName, lastName, role, token.AccessToken, HttpStatusCode.Created);
            // get user and check if it is in database
            var getUserRequest = new HttpRequestMessage(HttpMethod.Get, response.Headers.Location);
            getUserRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
            var getUserResponse = await HttpClient.SendAsync(getUserRequest);

            getUserResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
            var content = await getUserResponse.Content.ReadAsStringAsync();
            content.ShouldNotBeNullOrEmpty();
            var result = JsonConvert.DeserializeObject<UserInfoDto>(content);
            result.Email.ShouldBe(email);
            result.FirstName.ShouldBe(firstName);
            result.LastName.ShouldBe(lastName);
            result.Role.ShouldBe(role);

            // delete user
            var deleteUserRequest = new HttpRequestMessage(HttpMethod.Delete, response.Headers.Location);
            deleteUserRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
            var deleteUserResponse = await HttpClient.SendAsync(deleteUserRequest);
            deleteUserResponse.StatusCode.ShouldBe(HttpStatusCode.NoContent);
            // check if user is deleted
            var getUserRequest2 = new HttpRequestMessage(HttpMethod.Get, response.Headers.Location);
            getUserRequest2.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
            var getUserResponse2 = await HttpClient.SendAsync(getUserRequest2);
            getUserResponse2.StatusCode.ShouldBe(HttpStatusCode.NotFound);

            // clear database
            await db.QueryAsync(deleteUserQuery);
        }
        [Test]
        public async Task Will_return_all_users()
        {
            //await ResetDatabase();
            string email = "adam.kowalski@gmail.com";
            string password = "Password1";
            string firstName = "Adam";
            string lastName = "Kowalski";
            string role = "user";
            // ensure that user is not in database
            using var db = new SqlConnection(_connectionString);
            string deleteUserQuery = $"delete from [dbo].[User] where [dbo].[User].Email like '{email}'";
            await db.QueryAsync(deleteUserQuery);
            // login as admin and add user
            var token = await Login(AdminEmail, AdminPassword, "admin");
            var response = await AddUser(email, password, firstName, lastName, role, token.AccessToken, HttpStatusCode.Created);
            // get user and check if it is in database
            var getAllUserRequest = new HttpRequestMessage(HttpMethod.Get, Api+Urls.GetAllUsers);
            getAllUserRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
            var getUserResponse = await HttpClient.SendAsync(getAllUserRequest);

            getUserResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
            var content = await getUserResponse.Content.ReadAsStringAsync();
            content.ShouldNotBeNullOrEmpty();
            var result = JsonConvert.DeserializeObject<GetAllItemsWithCountDto<UserInfoDto>>(content);
            result.Items.Count.ShouldBeGreaterThan(4);
            result.TotalCount.ShouldBeGreaterThan(4);
            result.Page.ShouldBe(1);
            result.PageSize.ShouldBe(10);
            // clear database
            await db.QueryAsync(deleteUserQuery);
        }
        [Test]
        public async Task Will_return_user_by_id()
        {
            string email = "adam.kowalski@gmail.com";
            string password = "Password1";
            string firstName = "Adam";
            string lastName = "Kowalski";
            string role = "user";
            // ensure that user is not in database
            using var db = new SqlConnection(_connectionString);
            string deleteUserQuery = $"delete from [dbo].[User] where [dbo].[User].Email like '{email}'";
            await db.QueryAsync(deleteUserQuery);
            // login as admin and add user
            var token = await Login(AdminEmail, AdminPassword, "admin");
            var response = await AddUser(email, password, firstName, lastName, role, token.AccessToken, HttpStatusCode.Created);
            // get user and check if it is in database
            var getUserRequest = new HttpRequestMessage(HttpMethod.Get, response.Headers.Location);
            getUserRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
            var getUserResponse = await HttpClient.SendAsync(getUserRequest);

            getUserResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
            var content = await getUserResponse.Content.ReadAsStringAsync();
            content.ShouldNotBeNullOrEmpty();
            var result = JsonConvert.DeserializeObject<UserInfoDto>(content);
            result.Email.ShouldBe(email);
            result.FirstName.ShouldBe(firstName);
            result.LastName.ShouldBe(lastName);
            result.Role.ShouldBe(role);
            // clear database
            await db.QueryAsync(deleteUserQuery);
        }
        [Test]
        public async Task Will_update_user_as_admin()
        {
            string email = "adam.kowalski@gmail.com";
            string password = "Password1";
            string firstName = "Adam";
            string lastName = "Kowalski";
            string role = "user";
            // ensure that user is not in database
            using var db = new SqlConnection(_connectionString);
            string deleteUserQuery = $"delete from [dbo].[User] where [dbo].[User].Email like '{email}'";
            await db.QueryAsync(deleteUserQuery);
            // login as admin and add user
            var token = await Login(AdminEmail, AdminPassword, "admin");
            var response = await AddUser(email, password, firstName, lastName, role, token.AccessToken, HttpStatusCode.Created);
            // get user and check if it is in database
            var getUserRequest = new HttpRequestMessage(HttpMethod.Get, response.Headers.Location);
            getUserRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
            var getUserResponse = await HttpClient.SendAsync(getUserRequest);

            getUserResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
            var content = await getUserResponse.Content.ReadAsStringAsync();
            content.ShouldNotBeNullOrEmpty();
            var result = JsonConvert.DeserializeObject<UserInfoDto>(content);
            result.Email.ShouldBe(email);
            result.FirstName.ShouldBe(firstName);
            result.LastName.ShouldBe(lastName);
            result.Role.ShouldBe(role);
            // update user
            string newEmail = "adam.kowalski@gmail.com";
            string newFirstName = "Adam123";
            string newLastName = "Kowalski123";
            string newRole = "admin";
            var updateUserRequest = new HttpRequestMessage(HttpMethod.Patch, response.Headers.Location);
            updateUserRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
            var updateContent = JsonConvert.SerializeObject(
                new
                {
                    Email = newEmail,
                    FirstName = newFirstName,
                    LastName = newLastName,
                    Role = newRole
                });
            updateUserRequest.Content = new StringContent(updateContent);
            updateUserRequest.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var updateUserResponse = await HttpClient.SendAsync(updateUserRequest);
            updateUserResponse.StatusCode.ShouldBe(HttpStatusCode.NoContent);

            // get user and check if it is updated
            var getUserRequest2 = new HttpRequestMessage(HttpMethod.Get, response.Headers.Location);
            getUserRequest2.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
            var getUserResponse2 = await HttpClient.SendAsync(getUserRequest2);

            getUserResponse2.StatusCode.ShouldBe(HttpStatusCode.OK);
            var content2 = await getUserResponse2.Content.ReadAsStringAsync();
            content2.ShouldNotBeNullOrEmpty();
            var result2 = JsonConvert.DeserializeObject<UserInfoDto>(content2);
            result2.Email.ShouldBe(newEmail);
            result2.FirstName.ShouldBe(newFirstName);
            result2.LastName.ShouldBe(newLastName);
            result2.Role.ShouldBe(newRole);
            // clear database
            await db.QueryAsync(deleteUserQuery);
        }
        // user
        [Test]
        public async Task Will_not_add_user_as_user()
        {
            string NewUserEmail = Guid.NewGuid().ToString() + "@gmail.com";
            string NewUserPassword = "Password1.";
            string NewUserFirstName = Guid.NewGuid().ToString();
            string NewUserLastName = Guid.NewGuid().ToString();

            var token = await Login(UserEmail, UserPassword, "user");
            var _ = await AddUser(
                NewUserEmail,
                NewUserPassword,
                NewUserFirstName,
                NewUserLastName,
                "user",
                token.AccessToken,
                HttpStatusCode.Forbidden);
        }
        [Test]
        public async Task Will_not_delete_user_as_user()
        {
            string email = "adam.kowalski@gmail.com";
            string password = "Password1";
            string firstName = "Adam";
            string lastName = "Kowalski";
            string role = "user";
            // ensure that user is not in database
            using var db = new SqlConnection(_connectionString);
            string deleteUserQuery = $"delete from [dbo].[User] where [dbo].[User].Email like '{email}'";
            await db.QueryAsync(deleteUserQuery);
            // login as admin and add user
            var token = await Login(AdminEmail, AdminPassword, "admin");
            var response = await AddUser(email, password, firstName, lastName, role, token.AccessToken, HttpStatusCode.Created);
            // get user and check if it is in database
            var userToken = await Login(UserEmail, UserPassword, "user");
            var deleteUserRequest = new HttpRequestMessage(HttpMethod.Delete, response.Headers.Location);
            deleteUserRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userToken.AccessToken);
            var deleteUserReesponse = await HttpClient.SendAsync(deleteUserRequest);
            deleteUserReesponse.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
            // clear database
            await db.QueryAsync(deleteUserQuery);
        }
        [Test]
        public async Task Will_not_return_all_users_as_user()
        {
            var token = await Login(UserEmail, UserPassword, "user");
            var deleteUserRequest = new HttpRequestMessage(HttpMethod.Get, Api+Urls.GetAllUsers);
            deleteUserRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
            var deleteUserReesponse = await HttpClient.SendAsync(deleteUserRequest);
            deleteUserReesponse.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        }
        [Test]
        public async Task Will_not_return_user_by_id_as_user()
        {
            string email = "adam.kowalski@gmail.com";
            string password = "Password1";
            string firstName = "Adam";
            string lastName = "Kowalski";
            string role = "user";
            // ensure that user is not in database
            using var db = new SqlConnection(_connectionString);
            string deleteUserQuery = $"delete from [dbo].[User] where [dbo].[User].Email like '{email}'";
            await db.QueryAsync(deleteUserQuery);
            // login as admin and add user
            var adminToken = await Login(AdminEmail, AdminPassword, "admin");
            var response = await AddUser(email, password, firstName, lastName, role, adminToken.AccessToken, HttpStatusCode.Created);
            // get user and check if it is in database
            var userToken = await Login(UserEmail, UserPassword, "user");
            
            var getUserRequest = new HttpRequestMessage(HttpMethod.Get, response.Headers.Location);
            getUserRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userToken.AccessToken);
            var deleteUserReesponse = await HttpClient.SendAsync(getUserRequest);
            deleteUserReesponse.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
            // clear database
            await db.QueryAsync(deleteUserQuery);
        }
        [Test]
        public async Task Will_not_update_user_as_user()
        {

        }
        [Test]
        public async Task Will_return_himself_as_user()
        {

        }
        // unauthenticated
        [Test]
        public async Task Will_not_return_all_users_without_token()
        {
            var getAllUserRequest = new HttpRequestMessage(HttpMethod.Get, Api + Urls.GetAllUsers);
            var getAllUserReesponse = await HttpClient.SendAsync(getAllUserRequest);
            getAllUserReesponse.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }
        [Test]
        public async Task Will_not_return_user_by_id_without_token()
        {

        }
        [Test]
        public async Task Will_not_update_user_without_token()
        {

        }
        [Test]
        public async Task Will_not_return_himself_without_token()
        {

        }
        [Test]
        public async Task Will_not_delete_user_without_token()
        {
            string email = "adam.kowalski@gmail.com";
            string password = "Password1";
            string firstName = "Adam";
            string lastName = "Kowalski";
            string role = "user";
            // ensure that user is not in database
            using var db = new SqlConnection(_connectionString);
            string deleteUserQuery = $"delete from [dbo].[User] where [dbo].[User].Email like '{email}'";
            await db.QueryAsync(deleteUserQuery);
            // login as admin and add user
            var token = await Login(AdminEmail, AdminPassword, "admin");
            var response = await AddUser(email, password, firstName, lastName, role, token.AccessToken, HttpStatusCode.Created);
            // get user and check if it is in database
            var getUserRequest = new HttpRequestMessage(HttpMethod.Get, response.Headers.Location);
            getUserRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
            var getUserResponse = await HttpClient.SendAsync(getUserRequest);
            getUserResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
            var content = await getUserResponse.Content.ReadAsStringAsync();
            content.ShouldNotBeNullOrEmpty();
            var result = JsonConvert.DeserializeObject<UserInfoDto>(content);
            result.Email.ShouldBe(email);
            result.FirstName.ShouldBe(firstName);
            result.LastName.ShouldBe(lastName);
            result.Role.ShouldBe(role);
            // delete user without token
            var deleteUserRequest = new HttpRequestMessage(HttpMethod.Delete, response.Headers.Location);
            var deleteUserReesponse = await HttpClient.SendAsync(deleteUserRequest);
            deleteUserReesponse.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
            // clear database
            await db.QueryAsync(deleteUserQuery);
        }
        [Test]
        public async Task Will_not_add_user_without_token()
        {
            _ = await AddUser("adami.kowalski@gmail.com", "Password1", "Adam", "Kowalski", "user", string.Empty, HttpStatusCode.Unauthorized);
        }
    }
}
