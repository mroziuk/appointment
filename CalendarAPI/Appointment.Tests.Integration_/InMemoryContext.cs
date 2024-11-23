using Appointment.Data;
using Microsoft.EntityFrameworkCore;
namespace Appointment.Tests.Integration_
{
    public class InMemoryContext : CalendarContext
    {
        public InMemoryContext() : base()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseInMemoryDatabase(databaseName: "Test");
        }
    }
}
