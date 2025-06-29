using Core.Enums;

namespace Core.Params
{
    public class ServiceOrderParams : QueryParams
    {
        /// <summary>
        /// Filter by dentist/client ID.
        /// </summary>
        public int? ClientId { get; set; }

        /// <summary>
        /// Filter by service order status (e.g., Open, Finished).
        /// </summary>
        public OrderStatus? Status { get; set; }

        /// <summary>
        /// Filter by patient name (partial match, case-insensitive).
        /// </summary>
        public string? PatientName { get; set; }
    }
}
