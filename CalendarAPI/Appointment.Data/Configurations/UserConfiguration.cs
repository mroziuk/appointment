using Appointment.Data.Configurations;
using Appointment.Domain.Entities;
using Appointment.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Appointment.Data.Configurations
{
    internal sealed class UserConfiguration : BaseConfiguration<User>
    {
        internal override void ConfigureRelations(EntityTypeBuilder<User> builder)
        {
            builder
                .HasMany<Availabillity>(u => u.Availabillities)
                .WithOne(a => a.User)
                .HasForeignKey(a => a.UserId);
            builder
                .HasMany<Visit>(u => u.Visits)
                .WithOne(v => v.Therapist)
                .HasForeignKey(v => v.TherapistId);
        }

        internal override void ConfigureProperties(EntityTypeBuilder<User> builder)
        {
            builder
                .Property(x => x.Id)
                .IsRequired();
        }
    }
}