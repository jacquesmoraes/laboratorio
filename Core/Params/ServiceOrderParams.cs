using Core.Enums;

namespace Core.Params
{
    public class ServiceOrderParams : QueryParams
    {
        /// <summary>
        /// Filtro por ID do dentista/cliente.
        /// </summary>
        public int? ClientId { get; set; }

        /// <summary>
        /// Filtro por status da ordem de serviço (ex: Abertas, Finalizadas).
        /// </summary>
        public OrderStatus? Status { get; set; }

        /// <summary>
        /// Filtro por nome do paciente (parcial, insensível a maiúsculas).
        /// </summary>
        public string? PatientName { get; set; }
    }
}
