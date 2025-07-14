export interface ClientDashboardData {
  clientId: number;
  clientName: string;
  street?: string;
  number?: number; 
  complement?: string;
  neighborhood?: string;
  city?: string;
  phoneNumber?: string;
  totalInvoiced: number;
  totalPaid: number;
  balance: number;
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

export interface ClientInvoice {
  billingInvoiceId: number;
  invoiceNumber: string;
  createdAt: string;
  description?: string;
  totalServiceOrdersAmount: number; 
  totalPaid: number;
  outstandingBalance: number; 
  status: string;
  pdfDownloadUrl?: string;
  client?: any;
  serviceOrders?: any[];
  previousCredit?: number; 
  previousDebit?: number; 
}

export interface ServiceOrder {
  serviceOrderId: number;
  orderNumber: string;
  patientName: string;
  dateIn: string;
  lastMovementDate?: string;
  status: string;
  totalAmount: number; 
  currentSectorName?: string;
  clientName: string; 
  clientId: number; 
}

export interface ClientAreaDashboardData {
  dashboard: ClientDashboardData;
  recentOrders: ServiceOrder[];
  recentPayments: ClientPayment[];
  recentInvoices: ClientInvoice[];
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

// DataTable Interfaces
export interface TableColumn {
  key: string;
  label: string;
  sortable?: boolean;
  width?: string;
}

export interface PaginationInfo {
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  totalItems: number;
}