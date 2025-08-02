using Core.Models.Production;
using Core.Specifications;
using System.Linq.Expressions;

namespace Core.FactorySpecifications.ProductionSpecifications
{
    public class ShadeSpecification : BaseSpecification<Shade>
    {
        public ShadeSpecification ( ) { }

        public ShadeSpecification ( Expression<Func<Shade, bool>> criteria )
            : base ( criteria ) { }
    }

    public static class ShadeSpecs
    {
        public static ShadeSpecification All ( )
            => new ( );

        public static ShadeSpecification ById ( int id )
            => new ( s => s.Id == id );

        public static ShadeSpecification ByScaleId ( int scaleId )
            => new ( s => s.ScaleId == scaleId );

        public static ShadeSpecification AllWithScale ( )
        {
            var spec = new ShadeSpecification();
            spec.AddInclude ( s => s.Scale! );
            return spec;
        }
    }
}
