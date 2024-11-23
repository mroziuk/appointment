using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Common.Utils
{
    public class HttpHelper
    {
        public static class Urls
        {
            public readonly static string ApiGetAllUsers = "/api/users/all";
            public readonly static string GetAllUsers = "/users/all";
            public readonly static string GetUser = "/api/users/{0}";

            public readonly static string GetAllRooms = "api/rooms/all";
            public readonly static string GetRoom = "/api/rooms/{0}";

            public readonly static string ActivateAccount = "/api/identity/activate";
        }
    }
}
