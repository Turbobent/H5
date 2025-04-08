

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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User_Device>()
                .HasKey(ud => new { ud.UserId, ud.DeviceId });
        }

    }
}
