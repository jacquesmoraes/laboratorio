using Core.Models.WebSite;
using Core.Specifications;
using System.Linq.Expressions;

namespace Core.FactorySpecifications.WebSiteSpecifications
{
    public class WebsiteWorkTypeSpecification : BaseSpecification<WebsiteWorkType>
    {
        public WebsiteWorkTypeSpecification() { }

        public WebsiteWorkTypeSpecification(Expression<Func<WebsiteWorkType, bool>> criteria)
            : base(criteria) { }

        public static class WebsiteWorkTypeSpecs
        {
            public static WebsiteWorkTypeSpecification ByIdWithWorkType(int id)
            {
                var spec = new WebsiteWorkTypeSpecification(wt => wt.Id == id);
                spec.AddInclude(wt => wt.WorkType!);
                spec.AddInclude("WorkType.WorkSection");
                return spec;
            }

            public static WebsiteWorkTypeSpecification ActiveWithWorkType()
            {
                var spec = new WebsiteWorkTypeSpecification(wt => wt.IsActive);
                spec.AddInclude(wt => wt.WorkType! );
                spec.AddInclude("WorkType.WorkSection");
                spec.AddOrderBy(wt => wt.Order);
                return spec;
            }

            public static WebsiteWorkTypeSpecification AllWithWorkType()
            {
                var spec = new WebsiteWorkTypeSpecification();
                spec.AddInclude(wt => wt.WorkType! );
                spec.AddInclude("WorkType.WorkSection");
                spec.AddOrderBy(wt => wt.Order);
                return spec;
            }
        }
    }
}