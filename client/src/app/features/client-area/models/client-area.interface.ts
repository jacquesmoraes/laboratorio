export interface ClientDashboardData {
  clientId: number;
  clientName: string;
  street: string;
  number: string;
  complement?: string;
  neighborhood: string;
  city: string;
  phoneNumber: string;
  totalInvoiced: number;
  totalPaid: number;
  balance: number;
}

export interface ClientPayment {
  paymentId: number;
  paymentDate: string;
  amountPaid: number;
  paymentMethod: string;
  description: string;
  status: string;
}

export interface ClientInvoice {
  billingInvoiceId: number;
  invoiceNumber: string;
  createdAt: string;
  description: string;
  totalInvoiceAmount: number;
  totalPaid: number;
  balance: number;
  status: string;
  pdfDownloadUrl: string;
}

export interface ServiceOrder {
  serviceOrderId: number;
  orderNumber: string;
  patientName: string;
  dateIn: string;
  status?: string;
  totalAmount: number;
  currentSectorName?: string;
}

export interface PaginatedResponse<T> {
  data: T[];
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  totalItems: number;
}

export interface PaymentParams {
  pageNumber?: number;
  pageSize?: number;
  startDate?: string;
  endDate?: string;
}

export interface InvoiceParams {
  pageNumber?: number;
  pageSize?: number;
  status?: string;
  startDate?: string;
  endDate?: string;
  search?: string;
}

export interface OrderParams {
  pageNumber?: number;
  pageSize?: number;
  status?: string;
  search?: string;
}
