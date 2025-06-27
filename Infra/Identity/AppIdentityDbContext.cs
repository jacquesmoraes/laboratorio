using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infra.Identity
{
    public class AppIdentityDbContext ( DbContextOptions<AppIdentityDbContext> options ) 
        : IdentityDbContext<ApplicationUser, ApplicationRole, string>(options)
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            // Configurações específicas se necessário
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(u => u.DisplayName).IsRequired().HasMaxLength(100);
                entity.Property(u => u.AccessCode).HasMaxLength(10);
            });
            
          
        }
    }
}