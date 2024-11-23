using Appointment.Domain;
using Appointment.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Appointment.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CalendarContext _context;
        public DbSet<User> Users => _context.Users;
        public DbSet<Patient> Patients => _context.Patients;
        public DbSet<Room> Rooms => _context.Rooms;
        public DbSet<Visit> Visits => _context.Visits;
        public DbSet<Availabillity> Availabillities => _context.Availabillities;
        public DbSet<RefreshToken> RefreshTokens => _context.RefreshTokens;
        public UnitOfWork(CalendarContext context)
        {
            _context = context;
        }
        public int Commit()
        {
            return _context.SaveChanges();
        }
        public Task<int> SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
        public void Dispose()
        {
            _context.Dispose();
        }

        public DbContext GetContext()
        {
            return _context;
        }
    }
}
