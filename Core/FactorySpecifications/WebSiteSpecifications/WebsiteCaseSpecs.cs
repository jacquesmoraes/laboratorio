using Core.Models.WebSite;
using Core.Specifications;
using System.Linq.Expressions;

namespace Core.FactorySpecifications.WebSiteSpecifications
{
    public class WebsiteCaseSpecification : BaseSpecification<WebsiteCase>
    {
        public WebsiteCaseSpecification() { }

        public WebsiteCaseSpecification(Expression<Func<WebsiteCase, bool>> criteria)
            : base(criteria) { }

        public static class WebsiteCaseSpecs
        {
            public static WebsiteCaseSpecification ByIdWithImages(int id)
            {
                var spec = new WebsiteCaseSpecification(c => c.Id == id);
                spec.AddInclude(c => c.Images);
                spec.AddOrderBy(c => c.Order);
                return spec;
            }

            public static WebsiteCaseSpecification ActiveForHomepage()
            {
                var spec = new WebsiteCaseSpecification(c => c.IsActive);
                spec.AddInclude(c => c.Images);
                spec.AddOrderBy(c => c.Order);
                spec.ApplyPaging(0, 3); // Apenas 3 casos
                return spec;
            }

            public static WebsiteCaseSpecification AllWithImages()
            {
                var spec = new WebsiteCaseSpecification();
                spec.AddInclude(c => c.Images);
                spec.AddOrderBy(c => c.Order);
                return spec;
            }

            public static WebsiteCaseSpecification ActiveWithImages()
            {
                var spec = new WebsiteCaseSpecification(c => c.IsActive);
                spec.AddInclude(c => c.Images);
                spec.AddOrderBy(c => c.Order);
                return spec;
            }

            public static WebsiteCaseSpecification InactiveWithImages()
            {
                var spec = new WebsiteCaseSpecification(c => !c.IsActive);
                spec.AddInclude(c => c.Images);
                spec.AddOrderBy(c => c.Order);
                return spec;
            }
        }
    }
}