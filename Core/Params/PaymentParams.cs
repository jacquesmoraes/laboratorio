namespace Core.Params
{
    public class PaymentParams : QueryParams
    {
    
        /// <summary>
        /// Filtro por cliente (usado na dashboard do sistema).
        /// Na área do cliente, esse valor será fixado com base na claim.
        /// </summary>
        public int? ClientId { get; set; }

    }
}
