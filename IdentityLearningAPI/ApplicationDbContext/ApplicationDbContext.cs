using IdentityLearningAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityLearningAPI.ApplicationDbContext
{
    public class ApplicationDatabaseContext: IdentityDbContext<User>
    {
        public ApplicationDatabaseContext(DbContextOptions<ApplicationDatabaseContext> options): base(options)
        {

        }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<MeetingRoom> MeetingRooms { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}
