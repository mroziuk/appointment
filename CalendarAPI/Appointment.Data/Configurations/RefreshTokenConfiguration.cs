using Appointment.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Data.Configurations
{
    internal class RefreshTokenConfiguration : BaseConfiguration<RefreshToken>
    {
        internal override void ConfigureProperties(EntityTypeBuilder<RefreshToken> builder)
        {
            builder
                .Property(rt => rt.Id)
                .IsRequired();
        }

        internal override void ConfigureRelations(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.
                HasOne<User>(rt => rt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId);
        }
    }
}
