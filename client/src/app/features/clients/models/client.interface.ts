export interface Client {
  clientId: number;
  clientName: string;
  birthDate?: string;
  clientEmail?: string;
  cro?: string;
  cnpj?: string;
  clientCpf?: string;
  clientPhoneNumber?: string;
  billingMode: BillingMode;
  notes?: string;
  address: ClientAddress;
  tablePriceId?: number;
  tablePriceName?: string;
  isInactive: boolean;
  city?: string;
}

export interface ClientAddress {
  street: string;
  number: number;
  complement: string;
  cep: string;
  neighborhood: string;
  city: string;
}

export interface CreateClientDto {
  name: string;
  birthDate?: string;
  email?: string;
  cro?: string;
  cnpj?: string;
  cpf?: string;
  phoneNumber?: string;
  billingMode: BillingMode;
  address: ClientAddress;
  tablePriceId?: number;
}

export interface UpdateClientDto {
  clientId: number;
  clientName: string;
  clientEmail?: string;
  clientPhoneNumber?: string;
  clientCpf?: string;
  cro?: string;
  cnpj?: string;
  birthDate?: string;
  notes?: string;
  billingMode: BillingMode;
  tablePriceId: number;
  address: ClientAddress;
}

export interface ClientDetails extends Client {
  totalPaid: number;
  totalInvoiced: number;
  balance: number;
  serviceOrders: ServiceOrderShort[];
  payments: ClientPayment[];
}

export interface ServiceOrderShort {
  serviceOrderId: number;
  orderNumber: string;
  dateIn: string;
  patientName: string;
  status: string;
  orderTotal: number;
}

export interface QueryParams {
  pageNumber: number;
  pageSize: number;
  sort?: string;
  search?: string;
}

export interface Pagination<T> {
  pageNumber: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
  data: T[];
}

export interface ClientPayment {
  id: number;
  paymentDate: string;
  amountPaid: number;
  description?: string;
  clientId: number;
  clientName: string;
  billingInvoiceId?: number;
  invoiceNumber?: string;
}

export enum BillingMode {
  PerMonth = 0,
  PerServiceOrder = 1
}

export const BillingModeLabels: Record<BillingMode, string> = {
  [BillingMode.PerMonth]: 'Mensal',
  [BillingMode.PerServiceOrder]: 'Por fatura'
};