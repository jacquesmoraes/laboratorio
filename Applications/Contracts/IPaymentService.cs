namespace Applications.Contracts
{
    public interface IPaymentService : IGenericService<Payment>
    {

        Task<Payment> RegisterClientPaymentAsync ( CreatePaymentDto dto );
        Task<Pagination<ClientPaymentRecord>> GetPaginatedAsync ( PaymentParams p );
        Task<Pagination<ClientPaymentRecord>> GetPaginatedForClientAreaAsync ( PaymentParams parameters );

    }


}
