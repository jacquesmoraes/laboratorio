using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Core.Models.Billing;

namespace Infra.Data.Config
{
    public class BillingInvoiceConfiguration : IEntityTypeConfiguration<BillingInvoice>
    {
        public void Configure(EntityTypeBuilder<BillingInvoice> builder)
        {
            builder.HasMany(i => i.Payments)
                   .WithOne(p => p.BillingInvoice)
                   .HasForeignKey(p => p.BillingInvoiceId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
