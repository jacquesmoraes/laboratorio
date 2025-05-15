using Core.Models.Clients;
using Core.Models.Pricing;
using Core.Models.Production;
using Core.Models.ServiceOrders;
using Core.Models.Works;
using Microsoft.EntityFrameworkCore;

namespace Infra.Data
{
    public class ApplicationContext ( DbContextOptions options ) : DbContext( options )
    {
        public DbSet<Client> Clients { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<ClientPayment> ClientPayments { get; set; }
       

        public DbSet<TablePrice> TablePrices { get; set; }
        public DbSet<TablePriceItem> tablePriceItems { get; set; }

        public DbSet<Scale> Scales { get; set; }
        public DbSet<Shade> Shades { get; set; }


        public DbSet<OrderPayment> OrderPayments { get; set; }
        public DbSet<ProductionStage> ProductionStages { get; set; }
        public DbSet<ServiceOrder> ServiceOrders { get; set; }


        public DbSet<Work> Works { get; set; }
        public DbSet<WorkType> WorkTypes { get; set; }
        public DbSet<WorkSection> WorkSections { get; set; }

        protected override void OnModelCreating ( ModelBuilder modelBuilder )
        {
            base.OnModelCreating ( modelBuilder );
            modelBuilder.Entity<Client> ( ).OwnsOne ( c => c.Address );
        }

    }
}
