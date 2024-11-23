using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Domain.Entities
{
    public class RefreshToken : AuditableEntity
    {
        public int Id { get; init; }
        public string Token { get; private set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public DateTime ExpirationTime { get; set; }
        public DateTime? UpdatedAt { get; private set; }
        public RefreshToken(int userId, string token, DateTime expirationTime)
        {
            UserId = userId;
            Token = token;
            ExpirationTime = expirationTime;
            UpdatedAt = null;
        }
        public void Update(string token, DateTime expirationTime)
        {
            Token = token;
            ExpirationTime = expirationTime;
            UpdatedAt = DateTime.UtcNow;
        }
    }
    public static class RefreshTokenSpecifiactions
    {
        public static IQueryable<RefreshToken> NotExpired(this IQueryable<RefreshToken> query)
        {
            return query.Where(x => x.ExpirationTime >= DateTime.UtcNow);
        }
    }
}
