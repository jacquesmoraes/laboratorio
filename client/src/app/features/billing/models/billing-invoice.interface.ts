export type InvoiceStatus = 'Open' | 'PartiallyPaid' | 'Paid' | 'Cancelled' | 'Closed';

export interface BillingInvoice {
  billingInvoiceId: number;
  invoiceNumber: string;
  createdAt: string;
  description: string;
  clientId: number;
  client: ClientInvoice;
  serviceOrders: InvoiceServiceOrder[];
  totalServiceOrdersAmount: number;
  previousCredit: number;
  previousDebit: number;
  totalInvoiceAmount: number;
  pdfDownloadUrl?: string;
  status: InvoiceStatus;
}

export interface ClientInvoice {
  clientId: number;
  clientName: string;
  address: ClientAddress;
  phoneNumber?: string;
}

export interface ClientAddress {
  street: string;
  number: number;
  complement: string;
  cep: string;
  neighborhood: string;
  city: string;
}

export interface InvoiceServiceOrder {
  dateIn: string;
  orderCode: string;
  works: InvoiceWorkItem[];
  subtotal: number;
  patientName: string;
  finishedAt?: string;
}

export interface InvoiceWorkItem {
  workTypeName: string;
  quantity: number;
  priceUnit: number;
  priceTotal: number;
}

export interface CreateBillingInvoiceDto {
  clientId: number;
  serviceOrderIds: number[];
  description?: string;
}

export interface InvoiceParams {
  pageNumber: number;
  pageSize: number;
  sort?: string;
  search?: string;
  clientId?: number;
  startDate?: string;
  endDate?: string;
}

export interface Pagination<T> {
  pageNumber: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
  data: T[];
}