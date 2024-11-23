using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Appointment.Domain.Entities;
using Appointment.Data.Configurations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Appointment.Domain.Entities.Identity;
using Appointment.Data.SecretStore;

namespace Appointment.Data;

internal class DateOnlyConverter : ValueConverter<DateOnly, DateTime>
{
    public DateOnlyConverter()
        : base(d => d.ToDateTime(TimeOnly.MinValue),
            d => DateOnly.FromDateTime(d))
    { }
}
internal class TimeOnlyConverter : ValueConverter<TimeOnly, DateTime>
{
    public TimeOnlyConverter()
        : base(d => DateTime.MinValue.Add(d.ToTimeSpan()),
            d => TimeOnly.FromDateTime(d))
    { }
}
public class CalendarContext : DbContext
{
    private readonly IAzureKeyVaultReader kvReader;
    private readonly string connectionString;
    public CalendarContext(IAzureKeyVaultReader kvReader) : base()
    {
        this.kvReader = kvReader;
        var result = this.kvReader.TryGetSecretAsync(KeyVaultConsts.MsSqlConnectionStringName).Result;

        if(result.suceeded)
        {
            connectionString = result.secret;
        }
    }
    public CalendarContext()
    {
        
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if(connectionString != null)
        {
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
    //public CalendarContext(DbContextOptions<CalendarContext> options) : base(options) { }
    public DbSet<User> Users { get; init; }
    public DbSet<Availabillity> Availabillities { get; init; }
    public DbSet<Patient> Patients { get; init; }
    public DbSet<Room> Rooms { get; init; }
    public DbSet<Visit> Visits { get; init; }
    public DbSet<RefreshToken> RefreshTokens { get; init; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new PatientConfiguration());
        modelBuilder.ApplyConfiguration(new RoomConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new VisitConfiguration());
        modelBuilder.ApplyConfiguration(new AvailabillityConfiguration());
        modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());
    }
    protected override void ConfigureConventions(ModelConfigurationBuilder builder)
    {
        builder.Properties<DateOnly>()
            .HaveConversion<DateOnlyConverter>()
            .HaveColumnType("dateTime2");
        builder.Properties<TimeOnly>()
            .HaveConversion<TimeOnlyConverter>()
            .HaveColumnType("dateTime2");
        base.ConfigureConventions(builder);
    }
}
