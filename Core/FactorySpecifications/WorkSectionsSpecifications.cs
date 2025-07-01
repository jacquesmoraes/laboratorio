using Core.Models.Works;
using Core.Specifications;

namespace Core.FactorySpecifications
{
    public class WorkSectionSpecification : BaseSpecification<WorkSection>
    {
        public WorkSectionSpecification ( ) { }

        public WorkSectionSpecification ( int id ) : base ( x => x.Id == id ) { }
    }

    public static class WorkSectionSpecs
    {
        public static WorkSectionSpecification All ( ) => new ( );
        public static WorkSectionSpecification ById ( int id ) => new ( id );
    }

}
