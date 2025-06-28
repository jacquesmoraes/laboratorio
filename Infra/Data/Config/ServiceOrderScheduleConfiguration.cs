using Core.Models.Schedule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Data.Config
{
    public class ServiceOrderScheduleConfiguration : IEntityTypeConfiguration<ServiceOrderSchedule>
    {
        public void Configure ( EntityTypeBuilder<ServiceOrderSchedule> builder )
        {

            builder.ToTable ( "ServiceOrderSchedules" );

            builder.HasKey ( x => x.Id );

            builder.Property ( x => x.ScheduledDate )
                .HasColumnType ( "timestamptz" );

            builder.Property ( x => x.CreatedAt )
                .HasColumnType ( "timestamptz" )
                .HasDefaultValueSql ( "CURRENT_TIMESTAMP" );

            builder.Property ( x => x.IsDelivered )
                .IsRequired ( );

            builder.Property ( x => x.IsOverdue )
                .IsRequired ( );

            builder.Property ( x => x.DeliveryType )
                .HasConversion<int> ( )
                .HasColumnName ( "DeliveryType" )
                .IsRequired ( false ); // enum opcional

            builder.HasOne ( x => x.ServiceOrder )
                .WithMany ( )
                .HasForeignKey ( x => x.ServiceOrderId )
                .OnDelete ( DeleteBehavior.Cascade );

            builder.HasOne ( x => x.Sector )
                .WithMany ( )
                .HasForeignKey ( x => x.SectorId )
                .OnDelete ( DeleteBehavior.Restrict );
        
        }
}
}
