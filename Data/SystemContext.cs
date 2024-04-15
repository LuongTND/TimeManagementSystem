using Microsoft.EntityFrameworkCore;
using TimeTrackingSystem.Models;

namespace TimeTrackingSystem.Data
{
    public class SystemContext : DbContext
    {
        public SystemContext(DbContextOptions<SystemContext> options) : base(options) { }

        public DbSet<Role> Roles { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Models.Task> Tasks { get; set; }
        public DbSet<Meeting> Meetings { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupMember> GroupMembers { get; set; }
        public DbSet<Clock> Clocks { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(new List<Role>()
            {
                new Role {RoleID = 1, Name = "User"},
                new Role {RoleID = 2, Name = "Premium User"}
            });

            modelBuilder.Entity<GroupMember>()
                .HasKey(e => new
                {
                    e.AccountID,
                    e.GroupID
                });

            modelBuilder.Entity<Account>()
                .HasMany(e => e.JoinGroups)
                .WithOne(e => e.Account)
                .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.Entity<Group>()
                .HasMany(e => e.Members)
                .WithOne(e => e.Group)
                .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.Entity<Meeting>()
                .Property(e => e.MeetingCode)
                .HasDefaultValueSql("newid()");

            modelBuilder.Entity<Clock>()
                .HasOne(e => e.Account)
                .WithOne(e => e.Clock);
        }
    }
}
