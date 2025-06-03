using Core.Models.ServiceOrders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Data.Config
{
    public class ServiceOrderConfiguration : IEntityTypeConfiguration<ServiceOrder>
    {
        public void Configure ( EntityTypeBuilder<ServiceOrder> builder )
        {
            builder
               .HasOne ( o => o.BillingInvoice )
               .WithMany ( i => i.ServiceOrders )
               .HasForeignKey ( o => o.BillingInvoiceId )
               .OnDelete ( DeleteBehavior.SetNull );

            builder
                .HasMany ( o => o.Stages )
                .WithOne ( s => s.ServiceOrder )
                .HasForeignKey ( s => s.ServiceOrderId )
                .OnDelete ( DeleteBehavior.Cascade );
            builder
                .HasMany ( o => o.Works )
                .WithOne ( w => w.ServiceOrder )
                .HasForeignKey ( w => w.ServiceOrderId )
                .OnDelete ( DeleteBehavior.Cascade );
        }
    }
}
