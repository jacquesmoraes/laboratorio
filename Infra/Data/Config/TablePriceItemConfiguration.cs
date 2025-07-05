using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Core.Models.Pricing;

namespace Infra.Data.Config
{
    public class TablePriceItemConfiguration : IEntityTypeConfiguration<TablePriceItem>
    {
        public void Configure(EntityTypeBuilder<TablePriceItem> builder)
        {
            builder.HasKey(i => i.TablePriceItemId);

            builder
                .HasOne(i => i.TablePrice)
                .WithMany(p => p.Items)
                .HasForeignKey(i => i.TablePriceId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(i => i.WorkType)
                .WithMany()
                .HasForeignKey(i => i.WorkTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(i => i.Price)
                .HasColumnType("decimal(10,2)")
                .IsRequired();
        }
    }
}
