using System.Runtime.Serialization;

namespace Core.Enums
{
    public enum BillingMode
    {
        [EnumMember(Value = "per_month")]
        perMonth = 0,
        [EnumMember(Value = "per_service_order")]
        perServiceOrder = 1
    }
}
