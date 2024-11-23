using Appointment.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Appointment.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Runtime.CompilerServices;
using Appointment.Domain.DTO.User;
using Appointment.Domain.DTO;
using Microsoft.EntityFrameworkCore;
using Appointment.Api.Services;
using Appointment.Api.Attributes;
using Appointment.Api.Auth;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System.IdentityModel.Tokens.Jwt;
using Appointment.Domain;

namespace Appointment.Api.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : BaseController
    {
        private readonly IUnitOfWork _uow;

        public UserController(CalendarContext context, IUnitOfWork uow)
            : base(context)
        {
            _uow = uow;
        }
        // GET: api/users/{id}
        [HttpGet("{id}", Name ="GetUserById")]
        [AuthorizeApi(Policies.Admin)]
        public async Task<ActionResult<UserInfoDto>> GetUser(int? id)
        {
            var user = await _uow.Users.AsNoTracking().NotDeleted().WithIdAsync(id ?? GetUserId());
            return user == null ? NotFound() : Ok(user.Present());
        }
        // GET: api/users/all
        [HttpGet("all")]
        [AuthorizeApi(Policies.Admin)]
        public async Task<ActionResult<GetAllItemsWithCountDto<UserInfoDto>>> GetUsersAsync(int? page, int? pageSize,string? name, string? login)
        {
            var users = _uow.Users.NotDeleted().WithName(name).WithLogin(login);
            var presentedUsers = await users.Page(page, pageSize).PresentedOnListAsync();
            return new GetAllItemsWithCountDto<UserInfoDto>()
            {
                Items = presentedUsers,
                Page = page ?? UserSpecification.defaultPage,
                PageSize = pageSize ?? UserSpecification.defaultPageSize,
                TotalCount = await users.CountAsync(),
            };
        }
        // GET: api/users/me
        [HttpGet("me")]
        [AuthorizeApi(Policies.User)]
        public async Task<ActionResult<UserInfoDto>> GetUserMe()
        {
            var user = await _uow.Users.AsNoTracking().NotDeleted().WithIdAsync(GetUserId());
            return user == null ? NotFound() : Ok(user.Present());
        }
        // POST: api/users
        [HttpPost]
        [AuthorizeApi(Policies.Admin)]
        public async Task<ActionResult<UserInfoDto>> PostUser(AddUserDto dto)
        {
            var user = new User(dto, _uow.Users);
            _uow.Users.Add(user);
            await _uow.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }
        // DELETE: api/users/{id}
        [HttpDelete("{id}")]
        [AuthorizeApi(Policies.Admin)]
        public async Task<ActionResult<UserInfoDto>> DeleteUser(int id)
        {
            var user = await _uow.Users
                .Include(u => u.Visits)
                .Include(u => u.Availabillities)
                .NotDeleted()
                .WithIdAsync(id);
            if (user == null) return NotFound();
            user.MarkAsDeleted();
            await _uow.SaveChangesAsync();
            return NoContent();
        }
        // PATCH: api/users/{id}
        [HttpPatch("{id}")]
        [AuthorizeApi(Policies.Admin)]
        public async Task<ActionResult<UserInfoDto>> PatchUser(int id, [FromBody] UpdateUserDto dto)
        {
            var user = await _uow.Users.NotDeleted().WithIdAsync(id);
            if (user == null) return NotFound();
            user.Update(dto);
            await _uow.SaveChangesAsync();
            return NoContent();
        }
        // POST: api/users/activate/{id}
        [HttpPost("activate/{id}")]
        [AuthorizeApi(Policies.Admin)]
        public async Task<ActionResult> ActivateUser(int id)
        {
            var user = await _uow.Users.NotDeleted().WithIdAsync(id);
            if(user == null) return NotFound();
            user.Activate();
            await _uow.SaveChangesAsync();
            
            return Ok();
        }
    }
}
