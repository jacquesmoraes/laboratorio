using Core.Models.Production;
using Core.Specifications;

namespace Core.FactorySpecifications.ProductionSpecifications
{
    public static class ScaleSpecifications
    {
        public static BaseSpecification<Scale> All ( )
            => new( );

        public static BaseSpecification<Scale> ById ( int id )
            => new( s => s.Id == id );
    }

}
