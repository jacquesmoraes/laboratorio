using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Applications.Dtos.Clients
{
    public class ClientResponseDto
    {
        public int ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string? ClientEmail { get; set; }
        public string? ClientCpf { get; set; }
        public string? ClientPhoneNumber { get; set; }

        public int TablePriceId { get; set; }
        public string? TablePriceName { get; set; }

        public string? Street { get; set; }
        public int? Number { get; set; }
        public string? Complement { get; set; }
        public string? Neighborhood { get; set; }
        public string? City { get; set; }

        public int BillingMode { get; set; }

        public bool IsInactive { get; set; }
        public List<PatientDto> Patients { get; set; } = [];
        public List<ClientPaymentDto> Payments { get; set; } = [];
        public List<ServiceOrderDto> ServiceOrders { get; set; } = [];
    }
}

