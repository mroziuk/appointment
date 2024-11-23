using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Appointment.Data.Configurations
{
    internal abstract class BaseConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
        where TEntity : class
    {
        public void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.ToTable(typeof(TEntity).Name);
            ConfigureProperties(builder);
            ConfigureRelations(builder);
        }

        internal abstract void ConfigureRelations(EntityTypeBuilder<TEntity> builder);

        internal abstract void ConfigureProperties(EntityTypeBuilder<TEntity> builder);
    }
}