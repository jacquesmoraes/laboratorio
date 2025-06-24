using Core.Models.Clients;
using Core.Models.LabSettings;
using Core.Models.Payments;
using Core.Models.Pricing;
using Core.Models.Production;
using Core.Models.ServiceOrders;
using Core.Models.Works;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infra.Data
{
    public class ApplicationContext ( DbContextOptions<ApplicationContext> options ) : DbContext ( options )
    {
         public DbSet<Client> Clients => Set<Client>();

        public DbSet<Payment> ClientPayments => Set<Payment> ( );
        public DbSet<TablePrice> TablePrices => Set<TablePrice> ( );
        public DbSet<TablePriceItem> TablePriceItems => Set<TablePriceItem> ( );
        public DbSet<Scale> Scales => Set<Scale> ( );
        public DbSet<Shade> Shades => Set<Shade> ( );
        public DbSet<SystemSettings> SystemSettings => Set<SystemSettings> ( );
        public DbSet<ProductionStage> ProductionStages => Set<ProductionStage> ( );
        public DbSet<Sector> Sectors => Set<Sector> ( );
        public DbSet<ServiceOrder> ServiceOrders => Set<ServiceOrder> ( );
        public DbSet<Work> Works => Set<Work> ( );
        public DbSet<WorkType> WorkTypes => Set<WorkType> ( );
        public DbSet<WorkSection> WorkSections => Set<WorkSection> ( );

        protected override void OnModelCreating ( ModelBuilder modelBuilder )
        {
            base.OnModelCreating ( modelBuilder );
            modelBuilder.ApplyConfigurationsFromAssembly ( typeof ( ApplicationContext ).Assembly );
            foreach ( var entityType in modelBuilder.Model.GetEntityTypes ( ) )
            {
                foreach ( var property in entityType.GetProperties ( ) )
                {
                    if ( property.ClrType == typeof ( DateTime ) )
                    {
                        property.SetValueConverter ( new ValueConverter<DateTime, DateTime> (
                            v => v.ToUniversalTime ( ),
                            v => DateTime.SpecifyKind ( v, DateTimeKind.Utc ) ) );
                    }
                    if ( property.ClrType == typeof ( DateTime? ) )
                    {
                        property.SetValueConverter ( new ValueConverter<DateTime?, DateTime?> (
                            v => v.HasValue ? v.Value.ToUniversalTime ( ) : v,
                            v => v.HasValue ? DateTime.SpecifyKind ( v.Value, DateTimeKind.Utc ) : v ) );
                    }
                }
            }


        }

    }
}
