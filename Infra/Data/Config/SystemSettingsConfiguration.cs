using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Data.Config
{
    public class SystemSettingsConfiguration : IEntityTypeConfiguration<SystemSettings>
    {
        public void Configure ( EntityTypeBuilder<SystemSettings> builder )
        {
            builder.OwnsOne ( e => e.Address, addr =>
       {
           addr.Property ( a => a.Street ).HasColumnName ( "Address_Street" );
           addr.Property ( a => a.Cep ).HasColumnName ( "Address_Cep" );
           addr.Property ( a => a.Number ).HasColumnName ( "Address_Number" );
           addr.Property ( a => a.Complement ).HasColumnName ( "Address_Complement" );
           addr.Property ( a => a.Neighborhood ).HasColumnName ( "Address_Neighborhood" );
           addr.Property ( a => a.City ).HasColumnName ( "Address_City" );
       } );
        }
    }
}
