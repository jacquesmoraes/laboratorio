using Core.Models.LabSettings;
using Core.Specifications;

namespace Core.FactorySpecifications
{
    public class SystemSettingsByIdSpec ( int id ) : BaseSpecification<SystemSettings>( x => x.Id == id )
    {
    }
}
