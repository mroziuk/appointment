using Appointment.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Appointment.Domain
{
    public interface IUnitOfWork : IDisposable
    {
        public DbSet<User> Users { get; }
        public DbSet<Patient> Patients { get; }
        public DbSet<Room> Rooms { get; }
        public DbSet<Visit> Visits { get; }
        public DbSet<Availabillity> Availabillities { get;}
        public DbSet<RefreshToken> RefreshTokens { get; }

        DbContext GetContext();
        int Commit();
        Task<int> SaveChangesAsync();
    }
}
