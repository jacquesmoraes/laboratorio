using Core.Models.Pricing;
using Core.Specifications;
using System.Linq.Expressions;

namespace Core.FactorySpecifications
{
    public class TablePriceSpecification : BaseSpecification<TablePrice>
    {
        public TablePriceSpecification (bool includeRelations = false ) 
        {
             if (includeRelations)
        {
            AddInclude(x => x.Items);
            AddInclude(x => x.Clients);
                AddInclude("Items.WorkType");
        }
        }

        public TablePriceSpecification ( int id, bool includeRelations = false )
            : base ( x => x.Id == id )
        {
            if ( includeRelations )
                AddInclude ( x => x.Items );
            AddInclude("Items.WorkType");
            AddInclude ( x => x.Clients );
        }

        public TablePriceSpecification ( Expression<Func<TablePrice, bool>> criteria, bool includeRelations = false )
            : base ( criteria )
        {
            if ( includeRelations )
                AddInclude ( x => x.Items );
            AddInclude ( x => x.Clients );
            AddInclude("Items.WorkType");
        }
    }

    public static class TablePriceSpecs
    {
        public static TablePriceSpecification AllWithRelations ( ) => new (includeRelations:true ) ;
        public static TablePriceSpecification ById ( int id ) => new ( id );
        public static TablePriceSpecification ByIdWithRelations ( int id ) => new ( id, includeRelations: true );
    }

}
