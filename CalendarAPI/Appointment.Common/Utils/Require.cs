using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Common.Utils
{
    public class Require
    {
        public class RequireResult
        {
            public void OrError<TException>() where TException :Exception, new()
            {
                if(!satisfied)
                {
                    throw new TException();
                }
            }

            public void OrError<TException>(TException expection) where TException : Exception
            {
                if(!satisfied)
                {
                    throw expection;
                }
            }

            public void OrError<TException>(Func<TException> exceptionFactory) where TException : Exception
            {
                if(!satisfied)
                {
                    throw exceptionFactory();
                }
            }

            public void OrError(Exception expection)
            {
                if(!satisfied)
                {
                    throw expection;
                }
            }

            public void OrError(Func<Exception> exceptionFactory)
            {
                if(!satisfied)
                {
                    throw exceptionFactory();
                }
            }

            public void OrError(string message)
            {
                if(!satisfied)
                {
                    throw new Exception(message);
                }
            }

            public RequireResult If(bool condition)
            {
                return condition ? this : new RequireResult(true);
            }

            public RequireResult IfNot(bool condition)
            {
                return condition ? new RequireResult(true) : this;
            }



            public RequireResult(bool satisfied)
            {
                this.satisfied = satisfied;
            }

            private bool satisfied;
        }

        public static RequireResult IsTrue(bool condition)
        {
            return new RequireResult(condition);
        }

        public static RequireResult IsFalse(bool condition)
        {
            return new RequireResult(!condition);
        }

        public static RequireResult NotEmpty(object obj)
        {
            return new RequireResult(obj != null);
        }

        public static RequireResult ChangableTo<T>(object obj)
        {
            return NoExceptions(() => { Convert.ChangeType(obj, typeof(T)); });
        }

        public static RequireResult NotEmpty(string str)
        {
            return new RequireResult(!string.IsNullOrWhiteSpace(str));
        }

        public static RequireResult NotEmpty<TSource>(IEnumerable<TSource> collection)
        {
            return new RequireResult(collection != null && collection.Any());
        }

        public static RequireResult NotEmpty<TSource>(Nullable<TSource> nullable) where TSource : struct
        {
            return new RequireResult(nullable != null && nullable.HasValue);
        }

        public static RequireResult MoreThanZero(float number)
        {
            return new RequireResult(number > 0);
        }

        public static RequireResult MoreThanZero(double number)
        {
            return new RequireResult(number > 0);
        }

        public static RequireResult MoreThanZero(int number)
        {
            return new RequireResult(number > 0);
        }

        public static RequireResult IsZero(int number)
        {
            return new RequireResult(number == 0);
        }

        public static RequireResult AreEqual(object excepted, object actual)
        {
            return new RequireResult(excepted == null ? actual == null : excepted.Equals(actual));
        }

        public static RequireResult AreEqual(string excepted, string actual, StringComparison comparisonType)
        {
            return new RequireResult(excepted.Equals(actual, comparisonType));
        }

        public static RequireResult IsEmpty(object obj)
        {
            return new RequireResult(obj == null);
        }

        public static RequireResult IsEmpty(string str)
        {
            return new RequireResult(string.IsNullOrEmpty(str));
        }

        public static RequireResult IsEmpty<TSource>(IEnumerable<TSource> collection)
        {
            return new RequireResult(collection == null || !collection.Any());
        }

        public static RequireResult IsEmpty<TSource>(Nullable<TSource> nullable) where TSource : struct
        {
            return new RequireResult(nullable == null || !nullable.HasValue);
        }

        public static RequireResult AreNotEqual(object excepted, object actual)
        {
            return new RequireResult(!excepted.Equals(actual));
        }

        public static RequireResult NoExceptions(Action action)
        {
            try
            {
                action();
                return new RequireResult(true);
            }
            catch(Exception)
            {
                return new RequireResult(false);
            }
        }

        public static RequireResult IsEmailAddress(string emailAddress)
        {
            try
            {
                new MailAddress(emailAddress);
                return new RequireResult(true);
            }
            catch(FormatException)
            {
                return new RequireResult(false);
            }
            catch(ArgumentNullException)
            {
                return new RequireResult(false);
            }
        }

        public static RequireResult AreEmailAddresses(IEnumerable<string> addresses)
        {
            try
            {
                addresses.ToList().ForEach(adr => new MailAddress(adr));
                return new RequireResult(true);
            }
            catch(FormatException)
            {
                return new RequireResult(false);
            }
            catch(ArgumentNullException)
            {
                return new RequireResult(false);
            }
        }
    }
}
