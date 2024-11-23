using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Appointment.Common.Utils
{
    public static class PhoneUtils
    {
        public static bool IsPhoneNumber(string number)
        {
            string pattern = @"(?:([+]\d{1,4})[-.\s]?)?(?:[(](\d{1,3})[)][-.\s]?)?(\d{1,4})[-.\s]?(\d{1,4})[-.\s]?(\d{1,9})";
            return Regex.Match(number, pattern).Success;
        }
    }
}
