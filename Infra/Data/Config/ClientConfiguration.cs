using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Data.Config
{
    public class ClientConfiguration : IEntityTypeConfiguration<Client>
    {
        public void Configure ( EntityTypeBuilder<Client> builder )
        {
            builder.OwnsOne ( c => c.Address );



            builder.OwnsMany ( c => c.Patients, pb =>
            {
                pb.WithOwner ( ).HasForeignKey ( "ClientId" );
                // Cria uma coluna de ID interno auto incrementável por cliente
                pb.Property<int> ( "PatientInternalId" );
                pb.HasKey ( "ClientId", "PatientInternalId" );
                pb.Property ( p => p.Name ).IsRequired ( );
                pb.Property ( p => p.Notes );
                pb.ToTable ( "Patients" );
            } );

        }
    }
}
