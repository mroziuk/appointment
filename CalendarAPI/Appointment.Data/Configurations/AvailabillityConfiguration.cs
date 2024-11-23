using Appointment.Domain.Entities;
using Appointment.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Data.Configurations
{
    internal class AvailabillityConfiguration : BaseConfiguration<Availabillity>
    {
        internal override void ConfigureProperties(EntityTypeBuilder<Availabillity> builder)
        {
            builder
                .Property(a => a.Id)
                .IsRequired();
        }

        internal override void ConfigureRelations(EntityTypeBuilder<Availabillity> builder)
        {
            builder
                .HasOne<User>(a => a.User)
                .WithMany(u => u.Availabillities)
                .HasForeignKey(a => a.UserId);
        }
    }
}
