using Core.Models.Production;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Data.Config
{
    public class ShadeConfiguration : IEntityTypeConfiguration<Shade>
    {
        public void Configure(EntityTypeBuilder<Shade> builder)
        {
            builder.Property(s => s.color).IsRequired();
            builder.HasOne(s => s.Scale)
                   .WithMany(s => s.Colors)
                   .HasForeignKey(s => s.ScaleId);
        }
    }
}
