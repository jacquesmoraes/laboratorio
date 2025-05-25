using Core.Helpers;
using Core.Models.ServiceOrders;

namespace Core.Models.Clients
{
    public class ClientBalance
    {
        public decimal Credit { get; private set; }
        public decimal Debt { get; private set; }

        public decimal CurrentBalance => Credit - Debt;
        public void AddCredit ( decimal amount )
        {
            Guard.AgainstNegativeOrZero ( amount );
            Credit += amount;
        }
        public void AddDebt ( decimal amount )
        {
            Guard.AgainstNegativeOrZero ( amount );
            Debt += amount;
        }

        public void AddDebtFromOrder ( ServiceOrder so ) =>
            AddDebt ( so.OrderTotal );
    }
}
