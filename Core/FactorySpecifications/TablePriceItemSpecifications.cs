using Core.Models.Pricing;
using Core.Specifications;
using System.Linq.Expressions;

namespace Core.FactorySpecifications
{
    public class TablePriceItemSpecification : BaseSpecification<TablePriceItem>
    {
        public TablePriceItemSpecification ( ) { }

        public TablePriceItemSpecification ( int id, bool includeRelations = false )
            : base ( x => x.TablePriceItemId == id )
        {
            if ( includeRelations )
            {
                AddInclude ( x => x.TablePrice! );
            }
        }

        public TablePriceItemSpecification ( Expression<Func<TablePriceItem, bool>> criteria, bool includeRelations = false )
            : base ( criteria )
        {
            if ( includeRelations )
            {
               
                AddInclude ( x => x.TablePrice! );
            }
        }
    }

    public static class TablePriceItemSpecs
    {

        public static TablePriceItemSpecification All ( ) => new ( x => true, includeRelations: true );
        public static TablePriceItemSpecification ByIds(IEnumerable<int> ids, bool includeRelations = false)
            => new TablePriceItemSpecification(x => ids.Contains(x.TablePriceItemId), includeRelations);

        public static TablePriceItemSpecification ByIdWithRelations ( int id )
            => new ( id, includeRelations: true );
        public static TablePriceItemSpecification ByTablePriceId ( int tablePriceId ) 
            => new ( x => x.TablePriceId == tablePriceId, includeRelations: true );
    }

}
