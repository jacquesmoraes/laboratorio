using Core.Models.Works;
using Core.Specifications;

namespace Core.FactorySpecifications
{
    public class WorkTypeSpecification : BaseSpecification<WorkType>
    {
        public WorkTypeSpecification ( bool includeRelations = false )
        {
            if ( includeRelations )
                AddInclude ( x => x.WorkSection );
        }

        public WorkTypeSpecification ( int id, bool includeRelations = false )
            : base ( x => x.Id == id )
        {
            if ( includeRelations )
                AddInclude ( x => x.WorkSection );
        }
    }

    public static class WorkTypeSpecs
    {
        public static WorkTypeSpecification All ( ) => new ( includeRelations: true );
        public static WorkTypeSpecification ById ( int id ) => new ( id );
        public static WorkTypeSpecification ByIdWithRelations ( int id ) => new ( id, includeRelations: true );
    }

}
