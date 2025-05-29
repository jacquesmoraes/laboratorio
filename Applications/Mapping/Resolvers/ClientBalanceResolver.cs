using AutoMapper;
using Core.Models.Clients;
using Applications.Dtos.Clients;

namespace Applications.Mapping.Resolvers
{
    public class ClientBalanceResolver : IValueResolver<Client, ClientResponseDetailsDto, ClientBalanceDto>
    {
        public ClientBalanceDto Resolve(Client source, ClientResponseDetailsDto destination, ClientBalanceDto destMember, ResolutionContext context)
        {
            var balance = ClientBalance.Calculate(source);

            return new ClientBalanceDto
            {
                TotalPaid = balance.TotalPaid,
                TotalInvoiced = balance.TotalInvoiced,
                Credit = balance.Credit,
                Debit = balance.Debit                
            };
        }
    }
}
