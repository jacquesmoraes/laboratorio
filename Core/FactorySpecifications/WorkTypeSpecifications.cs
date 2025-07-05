using Core.Models.Works;
using Core.Specifications;
using System.Linq.Expressions;

namespace Core.FactorySpecifications
{
    public class WorkTypeSpecification : BaseSpecification<WorkType>
    {
        public WorkTypeSpecification(bool includeRelations = false)
        {
            if (includeRelations)
                AddInclude(x => x.WorkSection);
        }

        public WorkTypeSpecification(int id, bool includeRelations = false)
            : base(x => x.Id == id)
        {
            if (includeRelations)
                AddInclude(x => x.WorkSection);
        }

        public WorkTypeSpecification(Expression<Func<WorkType, bool>> criteria)
            : base(criteria)
        {
        }
    }

    public static class WorkTypeSpecs
    {
        public static WorkTypeSpecification All() => new(includeRelations: true);
        public static WorkTypeSpecification ById(int id) => new(id);
        public static WorkTypeSpecification ByIdWithRelations(int id) => new(id, includeRelations: true);

        public static WorkTypeSpecification ByIds(IEnumerable<int> ids)
            => new(w => ids.Contains(w.Id));
    }
}
