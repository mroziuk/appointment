using Appointment.Data;
using Microsoft.EntityFrameworkCore;

namespace Appointment.Api.Extensions
{
    public static class MigrationExtension
    {
        public static void ApplyMigrations(this IApplicationBuilder app)
        {
            using IServiceScope scope = app.ApplicationServices.CreateScope();
            using CalendarContext context = scope.ServiceProvider.GetRequiredService<CalendarContext>();
            context.Database.Migrate();
        }
    }
}
