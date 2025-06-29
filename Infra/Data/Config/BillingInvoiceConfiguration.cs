using Core.Models.Billing;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Data.Config
{
    public class BillingInvoiceConfiguration : IEntityTypeConfiguration<BillingInvoice>
    {
        public void Configure ( EntityTypeBuilder<BillingInvoice> builder )
        {
            builder.HasMany ( i => i.Payments )
                   .WithOne ( p => p.BillingInvoice )
                   .HasForeignKey ( p => p.BillingInvoiceId )
                   .OnDelete ( DeleteBehavior.Cascade );
        }
    }
}
