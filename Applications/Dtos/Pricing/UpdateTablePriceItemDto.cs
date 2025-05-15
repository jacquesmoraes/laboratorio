using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Applications.Dtos.Pricing
{
   public class UpdateTablePriceItemDto
    {
        public int Id { get; set; }
         public int TablePriceId { get; set; }
        public int WorkTypeId { get; set; }
        public decimal Price { get; set; }
    }
}
