

namespace API.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<User_Device> User_Devices { get; set; }
        public DbSet<SharedPassword> SharedPasswords { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure the Device-SharedPassword relationship
            modelBuilder.Entity<Device>()
                .HasOne(d => d.SharedPassword)
                .WithMany(sp => sp.Devices)
                .HasForeignKey(d => d.SharedPasswordId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

            // Configure other relationships as needed
            modelBuilder.Entity<User_Device>()
                .HasKey(ud => new { ud.UserId, ud.DeviceId });
        }
    }
}
