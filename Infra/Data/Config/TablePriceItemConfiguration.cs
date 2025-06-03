using Core.Models.Pricing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace Infra.Data.Config
{
    public class TablePriceItemConfiguration : IEntityTypeConfiguration<TablePriceItem>
    {
        public void Configure ( EntityTypeBuilder<TablePriceItem> builder )
        {
            builder   
                .HasOne ( i => i.TablePrice )
                .WithMany ( p => p.Items )
                .HasForeignKey ( i => i.TablePriceId )
                .OnDelete ( DeleteBehavior.Cascade );
        }
    }
}
