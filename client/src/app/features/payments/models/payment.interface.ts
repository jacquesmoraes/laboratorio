export interface Payment {
  id: number;
  paymentDate: string;
  amountPaid: number;
  description?: string;
  clientId: number;
  clientName: string;
  billingInvoiceId?: number;
  invoiceNumber?: string;
}

export interface CreatePaymentDto {
  clientId: number;
  amountPaid: number;
  description?: string;
  paymentDate: string;
}

export interface PaymentParams {
  pageNumber: number;
  pageSize: number;
  sort?: string;
  search?: string;
  clientId?: number;
  startDate?: string; 
  endDate?: string ;
}

export interface Pagination<T> {
  pageNumber: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
  data: T[];
}