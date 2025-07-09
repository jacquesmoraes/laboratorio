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

        /// <summary>
        /// Filter by client name (partial match, case-insensitive).
        /// </summary>
        public string? ClientName { get; set; }

         /// <summary>
        /// When true, excludes finished service orders from the results.
        /// </summary>
        public bool ExcludeFinished { get; set; } = false;
        public bool ExcludeInvoiced { get; set; } = false; 
    }
}
