using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace Appointment.Api.Attributes
{
    public class AuthorizeApi : AuthorizeAttribute
    {
        public AuthorizeApi()
        {
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme;
        }

        public AuthorizeApi(string policy)
            : base(policy)
        {
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme;
        }
    }
}
