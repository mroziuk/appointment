using Appointment.Api;
using Appointment.Data;
using Appointment.Domain.DTO;
using Appointment.Domain.DTO.Availability;
using Appointment.Domain.DTO.Identity;
using Appointment.Domain.DTO.Patient;
using Appointment.Domain.DTO.Room;
using Appointment.Domain.DTO.Visit;
using Appointment.Domain.Entities.Identity;
using Appointment.Domain.Services.Auth;
using Appointment.Tests.Integration;
using Appointment.Tests.Unit.ObjectMothers;
using Azure.Core;
using Dapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Respawn;
using Shouldly;
using System.Drawing;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using static Appointment.Api.Extensions.CreateInitDataExtension;
namespace Appointment.Tests.EndToEnd
{
    public abstract class TestBaseE2E //: TestBase
    {
        protected HttpClient HttpClient { get; }
        protected readonly WebApplicationFactory<Program> _factory;
        protected const string Api = "https://appointment-app.azurewebsites.net/api";
        protected const string _connectionString = "Server=tcp:appointment-server.database.windows.net,1433;Initial Catalog=appointment-sql;Persist Security Info=False;User ID=mroziuk;Password=Krokodyle1.;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        protected readonly CalendarContext _context;
        protected TestBaseE2E() : base()
        {
            //using var scope = _factory.Services.CreateScope();
            //this.HttpClient = _factory.CreateClient();

            //var scopeService = scope.ServiceProvider;
            //_context = scopeService.GetRequiredService<CalendarContext>();

            //_context.Database.EnsureCreated();
            //CreateInitUsersAndAdmins(_context).Wait();
            //_context.SaveChanges();

            this.HttpClient = new HttpClient();
        }
        protected async Task ResetDatabase()
        {
            using var scope = _factory.Services.CreateScope();
            var scopeService = scope.ServiceProvider;
            var context = scopeService.GetRequiredService<CalendarContext>();
            //await _respawner.ResetAsync(_connectionString);
            await Register(SuperAdminEmail, SuperAdminPassword, "superadmin", "super", "admin");
            await Register(AdminEmail, AdminPassword, "admin", "admin", "admin");
            await Register(UserEmail, UserPassword, "user", "user", "user");
            context.Users.Where(u => u.Email.Equals(SuperAdminEmail)).Single().IsActive = true;
            context.Users.Where(u => u.Email.Equals(AdminEmail)).Single().IsActive = true;
            context.Users.Where(u => u.Email.Equals(UserEmail)).Single().IsActive = true;
            await context.SaveChangesAsync();
        }
        protected async Task CreateInitUsersAndAdmins(CalendarContext context)
        {
            if(!context.Users.Any(x => x.Email.Equals(SuperAdminEmail)))
            {
                await Register(SuperAdminEmail, SuperAdminPassword, "superadmin", "super", "admin");
            }
            if(!context.Users.Any(x => x.Email.Equals(AdminEmail)))
            {
                await Register(AdminEmail, AdminPassword, "admin", "admin", "admin");
            }
            if(!context.Users.Any(x => x.Email.Equals(UserEmail)))
            {
                await Register(UserEmail, UserPassword, "user", "user", "user");
            }
            // activate users
            var u = await context.Users.Where(u => u.Email.Equals(UserEmail)).FirstOrDefaultAsync();
            if(u != null) u!.IsActive = true;
            var a = await context.Users.Where(u => u.Email.Equals(AdminEmail)).FirstOrDefaultAsync();
            if(a != null) a!.IsActive = true;
            var s = await context.Users.Where(u => u.Email.Equals(SuperAdminEmail)).FirstOrDefaultAsync();
            if(s != null) s!.IsActive = true;
            await _context.SaveChangesAsync();
        }
        protected async Task<HttpResponseMessage> Register(string email, string password, string role, string firstName, string lastName)
        {
            await DeletedIfExist(email);
            using var request = new HttpRequestMessage(HttpMethod.Post, $"{Api}/identity/register");
            var body = new SignUpDto(email, email, password, firstName, lastName, role, null);
            var bodyString = JsonConvert.SerializeObject(body);
            request.Content = new StringContent(bodyString, Encoding.UTF8, "application/json");

            var response = await HttpClient.SendAsync(request);
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
            return response;
        }
        protected async Task DeletedIfExist(string email)
        {
            using var db = new SqlConnection(_connectionString);
            await db.QueryAsync($"delete from [dbo].[User] where [dbo].[User].Email = '{email}'");

        }
        protected async Task Activate(int id, string accessToken, HttpStatusCode expectedCode)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, $"{Api}/users/activate/{id}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await HttpClient.SendAsync(request);
            response.StatusCode.ShouldBe(expectedCode);
        }
        protected async Task<HttpResponseMessage> RegisterWithResponse(string emailPassword, string role, string firstName, string lastName)
        {
            throw new NotImplementedException();
        }
        //protected async Task<AuthTokenDto> Login(string email, string password, string expectedRole)
        //{
        //    using var request = new HttpRequestMessage(HttpMethod.Post, $"{Api}/identity/login");
        //    var body = new SignInDto(email, password);
        //    var bodyString = JsonConvert.SerializeObject(body);
        //    request.Content = new StringContent(bodyString, Encoding.UTF8, "application/json");

        //    var response = await HttpClient.SendAsync(request);
        //    response.StatusCode.ShouldBe(HttpStatusCode.OK);
        //    response.Content.ShouldNotBeNull();

        //    var content = await response.Content.ReadAsStringAsync();
        //    var result = JsonConvert.DeserializeObject<AuthTokenDto>(content);

        //    result.ShouldNotBeNull();
        //    result.Role.ShouldBe(expectedRole);
        //    result.AccessToken.ShouldNotBeNull();
        //    result.RefreshToken.ShouldNotBeNull();

        //    return result;
        //}
        protected async Task<AuthTokenDto> Login(string email, string password, string expectedRole)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, $"{Api}/identity/login");
            var body = new SignInDto(email, password);
            var bodyString = JsonConvert.SerializeObject(body);
            request.Content = new StringContent(bodyString, Encoding.UTF8, "application/json");

            var response = await HttpClient.SendAsync(request);
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            response.Content.ShouldNotBeNull();

            var token = await response.Content.ReadAsStringAsync();
            token.ShouldNotBeNullOrWhiteSpace();
            return new AuthTokenDto() { AccessToken = token};
        }
        protected async Task<AuthTokenDto> RefreshToken(string refreshToken)
        {
            throw new NotImplementedException();
        }
        protected async Task<HttpResponseMessage> GetUserMe(string accessToken, HttpStatusCode expectedStatus)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, $"{Api}/users/me");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await this.HttpClient.SendAsync(request);
            response.StatusCode.ShouldBe(expectedStatus);

            return response;
        }
        protected async Task<HttpResponseMessage> AddRoom(string name, string accessToken, HttpStatusCode expectedStatus)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, $"{Api}/rooms");

            var body = new AddRoomDto() { Name = name};
            request.Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await this.HttpClient.SendAsync(request);
            response.StatusCode.ShouldBe(expectedStatus);

            return response;
        }
        protected async Task<HttpResponseMessage> AddAvailability(int userId,DayOfWeek day, TimeOnly start, TimeOnly end, string accessToken, HttpStatusCode expectedStatus)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, $"{Api}/availability");

            var body = new AddAvailabilityDto() { UserId = userId, DayOfWeek = day, Start = start, End = end, ActiveFrom = DateOnly.MinValue, ActiveTo = DateOnly.MaxValue };
            request.Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await this.HttpClient.SendAsync(request);
            response.StatusCode.ShouldBe(expectedStatus);
            return response;
        }
        protected async Task<HttpResponseMessage> AddPatient(string firstName, string lastName, string phone, string accessToken, HttpStatusCode expectedStatus)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, $"{Api}/patients");

            var body = new AddPatientDto() { FirstName = firstName, LastName = lastName, Phone = phone };
            request.Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await this.HttpClient.SendAsync(request);
            response.StatusCode.ShouldBe(expectedStatus);
            return response;
        }
        protected async Task<HttpResponseMessage> AddUser(string login, string password,string firstName, string lastName, string role,string accessToken, HttpStatusCode expectedStatus)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, $"{Api}/users");

            var body = new AddUserDto()
            {
                Login = login,
                Password = password,
                Role = role,
                FirstName = firstName,
                LastName = lastName,
                Email = login
            };
            request.Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await this.HttpClient.SendAsync(request);
            response.StatusCode.ShouldBe(expectedStatus);
            return response;
        }
        public async Task<HttpResponseMessage> AddVisit(int userId, int patientId, int roomId, DateTime date, TimeSpan duration, string accessToken, HttpStatusCode expectedStatus)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, $"{Api}/visits");

            var body = new AddVisitDto() { TherapistId = userId, PatientId = patientId, RoomId = roomId, DateStart = date, DateEnd = date.Add(duration)};
            request.Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await this.HttpClient.SendAsync(request);
            response.StatusCode.ShouldBe(expectedStatus);
            return response;
        }
        public async Task<HttpResponseMessage> DeleteVisit(int visitId, string accessToken, HttpStatusCode expectedStatus)
        {
            using var request = new HttpRequestMessage(HttpMethod.Delete, $"{Api}/visits/{visitId}");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await this.HttpClient.SendAsync(request);
            response.StatusCode.ShouldBe(expectedStatus);
            return response;
        }

        #region deleteQueries
        protected async Task DeleteRoomByName(string name)
        {
            using var db = new SqlConnection(_connectionString);
            await db.QueryAsync($"delete from [dbo].[Room] where [dbo].[Room].Name = '{name}'");
        }
        protected async Task DeletePatientByLastName(string lastName)
        {
            using var db = new SqlConnection(_connectionString);
            await db.QueryAsync($"delete from [dbo].[Patient] where [dbo].[Patient].LastName = '{lastName}'");
        }

        #endregion
    }
}
