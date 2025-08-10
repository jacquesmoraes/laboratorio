using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Data.Config
{
    public class WorkSectionConfiguration : IEntityTypeConfiguration<WorkSection>
    {
        public void Configure ( EntityTypeBuilder<WorkSection> builder )
        {
            builder
                .HasMany ( ws => ws.WorkTypes )
                .WithOne ( wt => wt.WorkSection )
                .HasForeignKey ( wt => wt.WorkSectionId )
                .OnDelete ( DeleteBehavior.Cascade ); 
        }
    }
}