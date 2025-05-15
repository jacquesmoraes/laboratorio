using Core.Models.Pricing;
using Core.Specifications;
using System.Linq.Expressions;

namespace Core.FactorySpecifications
{
    public class TablePriceSpecification : BaseSpecification<TablePrice>
    {
        public TablePriceSpecification ( ) { }

        public TablePriceSpecification ( int id, bool includeRelations = false )
            : base ( x => x.Id == id )
        {
            if ( includeRelations )
                AddInclude ( x => x.Items );
        }

        public TablePriceSpecification ( Expression<Func<TablePrice, bool>> criteria, bool includeRelations = false )
            : base ( criteria )
        {
            if ( includeRelations )
                AddInclude ( x => x.Items );
        }
    }

    public static class TablePriceSpecs
    {
        public static TablePriceSpecification All ( ) => new ( );
        public static TablePriceSpecification ById ( int id ) => new ( id );
        public static TablePriceSpecification ByIdWithRelations ( int id ) => new ( id, includeRelations: true );
    }

}
