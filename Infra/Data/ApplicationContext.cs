using Core.Models.Billing;
using Core.Models.Clients;
using Core.Models.Payments;
using Core.Models.Pricing;
using Core.Models.Production;
using Core.Models.ServiceOrders;
using Core.Models.Works;
using Microsoft.EntityFrameworkCore;

namespace Infra.Data
{
    public class ApplicationContext ( DbContextOptions options ) : DbContext ( options )
    {
        public DbSet<Client> Clients { get; set; }

        public DbSet<Payment> ClientPayments { get; set; }
        public DbSet<TablePrice> TablePrices { get; set; }
        public DbSet<TablePriceItem> TablePriceItems { get; set; }
        public DbSet<Scale> Scales { get; set; }
        public DbSet<Shade> Shades { get; set; }

        public DbSet<ProductionStage> ProductionStages { get; set; }
        public DbSet<Sector> Sectors { get; set; }
        public DbSet<ServiceOrder> ServiceOrders { get; set; }
        public DbSet<Work> Works { get; set; }
        public DbSet<WorkType> WorkTypes { get; set; }
        public DbSet<WorkSection> WorkSections { get; set; }

        protected override void OnModelCreating ( ModelBuilder modelBuilder )
        {
            base.OnModelCreating ( modelBuilder );

            modelBuilder.Entity<Client> ( ).OwnsOne ( c => c.Address );


            modelBuilder.Entity<Client> ( b =>
            {
                b.OwnsMany ( c => c.Patients, pb =>
                {
                    pb.WithOwner ( ).HasForeignKey ( "ClientId" );
                    // Cria uma coluna de ID interno auto incrementável por cliente
                    pb.Property<int> ( "PatientInternalId" );
                    pb.HasKey ( "ClientId", "PatientInternalId" );
                    pb.Property ( p => p.Name ).IsRequired ( );
                    pb.Property ( p => p.Notes );
                    pb.ToTable ( "Patients" );
                } );
            } );

            modelBuilder.Entity<BillingInvoice> ( )
                .HasMany ( i => i.Payments )
                .WithOne ( p => p.BillingInvoice )
                .HasForeignKey ( p => p.BillingInvoiceId )
                .OnDelete ( DeleteBehavior.Cascade );


            modelBuilder.Entity<ServiceOrder> ( )
               .HasOne ( o => o.BillingInvoice )
               .WithMany ( i => i.ServiceOrders )
               .HasForeignKey ( o => o.BillingInvoiceId )
               .OnDelete ( DeleteBehavior.SetNull );

            modelBuilder.Entity<ServiceOrder> ( )
                .HasMany ( o => o.Stages )
                .WithOne ( s => s.ServiceOrder )
                .HasForeignKey ( s => s.ServiceOrderId )
                .OnDelete ( DeleteBehavior.Cascade );
            modelBuilder.Entity<ServiceOrder> ( )
    .HasMany ( o => o.Works )
    .WithOne ( w => w.ServiceOrder )
    .HasForeignKey ( w => w.ServiceOrderId )
    .OnDelete ( DeleteBehavior.Cascade );

            modelBuilder.Entity<TablePriceItem> ( )
    .HasOne ( i => i.TablePrice )
    .WithMany ( p => p.Items )
    .HasForeignKey ( i => i.TablePriceId )
    .OnDelete ( DeleteBehavior.Cascade );



        }

    }
}
