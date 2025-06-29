using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Data.Config
{
    public class ScaleConfiguration : IEntityTypeConfiguration<Scale>
    {
        public void Configure ( EntityTypeBuilder<Scale> builder )
        {
            builder.Property ( s => s.Name ).IsRequired ( );

            builder.HasMany ( s => s.Colors )
                   .WithOne ( c => c.Scale )
                   .HasForeignKey ( c => c.ScaleId )
                   .OnDelete ( DeleteBehavior.Cascade );
        }
    }
}
