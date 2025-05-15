namespace Core.Models.Clients
{
    public class ClientBalance
    {
        public decimal Credit { get; private set; } = 0;
        public decimal Debt { get; private set; } = 0;


        public void AddCredit ( decimal amount )
        {
            if ( amount <= 0 )
                throw new ArgumentException ( "Invalid Credit." );
            Credit += amount;
        }

        public void AddDebt ( decimal amount )
        {
            if ( amount <= 0 )
                throw new ArgumentException ( "Invalid Debt." );
            Debt += amount;
        }

        public void PayDebt ( decimal amount )
        {
            if ( amount <= 0 )
                throw new ArgumentException ( "Invalid payment." );

            if ( amount > Debt )
            {
                var leftover = amount - Debt;
                Debt = 0;
                Credit += leftover;
            }
            else
            {
                Debt -= amount;
            }
        }
    }
}
