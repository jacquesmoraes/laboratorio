namespace Infra.Identity
{
    public class AppIdentityDbContext ( DbContextOptions<AppIdentityDbContext> options )
        : IdentityDbContext<ApplicationUser, ApplicationRole, string> ( options )
    {
        public DbSet<UserLoginHistory> UserLoginHistories => Set<UserLoginHistory>();

        protected override void OnModelCreating ( ModelBuilder builder )
        {
            base.OnModelCreating ( builder );

            // Configurações específicas se necessário
            builder.Entity<ApplicationUser> ( entity =>
            {
                entity.Property ( u => u.DisplayName ).IsRequired ( ).HasMaxLength ( 100 );
                entity.Property ( u => u.AccessCode ).HasMaxLength ( 10 );
            } );

            builder.Entity<UserLoginHistory>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.IpAddress).HasMaxLength(45);
                entity.Property(e => e.UserAgent).HasMaxLength(512);
                entity.HasIndex(e => new { e.UserId, e.LoginAt });

                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

        }
    }
}

