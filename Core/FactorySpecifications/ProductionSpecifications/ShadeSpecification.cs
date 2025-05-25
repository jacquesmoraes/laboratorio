using Core.Models.Production;
using Core.Specifications;

namespace Core.FactorySpecifications.ProductionSpecifications
{
     public static class ShadeSpecifications
    {
        public static BaseSpecification<Shade> All()
            => new();

        public static BaseSpecification<Shade> ById(int id)
            => new(s => s.Id == id);

        public static BaseSpecification<Shade> ByScaleId(int scaleId)
            => new(s => s.ScaleId == scaleId);
    }

}
