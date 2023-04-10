using IdentityLearningAPI.Enums;
using IdentityLearningAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Primitives;

namespace IdentityLearningAPI.ApplicationDbContext
{
    public class ApplicationDatabaseContext: IdentityDbContext<User>
    {
        public ApplicationDatabaseContext(DbContextOptions<ApplicationDatabaseContext> options): base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserToken>()
                .Property(x => x.TokenType)
                .HasConversion(new EnumToStringConverter<TokenType>())
                .HasMaxLength(50);
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<MeetingRoomBooking> MeetingRoomBookings { get; set; }
        public DbSet<MeetingRoom> MeetingRooms { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<UserToken> UserTokens { get; set; }
        public DbSet<NewUserInvitation> NewUserInvitations { get; set; }
    }
}
