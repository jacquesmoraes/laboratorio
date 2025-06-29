namespace Core.Params
{
    public class PaymentParams : QueryParams
    {
        /// <summary>
        /// Filter by client (used in the system dashboard).
        /// In the client area, this value will be fixed based on the claim.
        /// </summary>
        public int? ClientId { get; set; }
    }
}
