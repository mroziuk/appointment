using Appointment.Data;
using Appointment.Domain.Entities.Identity;
using Appointment.Domain.Exceptions.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Appointment.Api.Controllers
{
    public class BaseController : ControllerBase
    {
        protected readonly CalendarContext _context;

        public BaseController(CalendarContext context)
        {
            _context = context;
        }
        protected int GetUserId()
        {
            if(!User.HasClaim(claim => claim.Type == ClaimTypes.NameIdentifier))
            {
                throw new NoIdClaimException();
            }

            var idString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(idString);
        }
        protected bool IsSuperAdmin() => User.IsInRole(Role.SuperAdmin);
        protected bool IsAdmin() => User.IsInRole(Role.Admin) || User.IsInRole(Role.SuperAdmin);
        protected bool IsUser() => User.IsInRole(Role.User);
    }
}
