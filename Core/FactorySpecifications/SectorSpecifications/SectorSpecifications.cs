using Core.Models.ServiceOrders;
using Core.Specifications;
using System.Linq.Expressions;

namespace Core.FactorySpecifications.SectorSpecifications
{
    public class SectorSpecification : BaseSpecification<Sector>
    {
        public SectorSpecification ( ) { }

        public SectorSpecification ( Expression<Func<Sector, bool>> criteria )
            : base ( criteria ) { }

        public SectorSpecification ( int id )
            : base ( x => x.SectorId == id ) { }

        public SectorSpecification ( string? name )
            : base ( x => name == null || x.Name.Contains ( name ) ) { }

        public static class SectorSpecs
        {
            public static SectorSpecification All ( )
            {
                var spec = new SectorSpecification();
                return spec;
            }

            public static SectorSpecification ById ( int id )
            => new ( x => x.SectorId == id );

            public static SectorSpecification ByName ( string name )
            {
                return new SectorSpecification ( name );
            }
        }
    }
}
