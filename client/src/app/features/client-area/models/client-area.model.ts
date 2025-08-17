export type ClientAreaOrderStatus = 'Production' | 'TryIn' | 'Finished';

export type InvoiceStatus = 'Open' | 'PartiallyPaid' | 'Paid' | 'Cancelled' | 'Closed';

export interface ClientDashboard {
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

export interface ClientAreaInvoice {
  billingInvoiceId: number;
  invoiceNumber: string;
  createdAt: string;
  description?: string;
  status: InvoiceStatus;
  totalInvoiceAmount: number;
}
export interface ClientAreaServiceOrder {
  serviceOrderId: number;
  orderNumber: string;
  dateIn: string;
  patientName: string;
  orderTotal: number;
  status: ClientAreaOrderStatus;
}


export interface ServiceOrderParams {
  pageNumber: number;
  pageSize: number;
  sort?: string;
  search?: string;
  status?: ClientAreaOrderStatus;
  clientId?: number;
}
export const orderStatusLabels = {
  Production: 'Em Produção',
  TryIn: 'Em Prova',
  Finished: 'Finalizado',
} satisfies Record<ClientAreaOrderStatus, string>;

export const orderStatusValues = Object.keys(orderStatusLabels) as ClientAreaOrderStatus[];

export const invoiceStatusLabels = {
  Open: "Aberto",
  PartiallyPaid: 'Parcialmente Pago',
  Paid: 'Pago',
  Cancelled: 'Cancelado',
  Closed: 'Fechado',
} satisfies Record<InvoiceStatus, string>;
export interface ClientAreaServiceOrderDetails {
  serviceOrderId: number;
  orderNumber: string;
  dateIn: string;
  dateOut?: string | null;
  patientName: string;
  status: ClientAreaOrderStatus;
  orderTotal: number;

  billingInvoiceNumber?: string | null;
  billingInvoiceId?: number | null;

  works: ClientAreaWork[];
  stages: ClientAreaStage[];
}

export interface ClientAreaWork {
  workTypeId: number;
  workTypeName: string;
  quantity: number;
  priceUnit: number;
  shadeColor: string;
  scaleName: string;
  notes?: string | null;
}

export interface ClientAreaStage {
  sectorId: number;
  sectorName: string;
  dateIn: string;
  dateOut?: string | null;
}


export const invoiceStatusValues = Object.keys(invoiceStatusLabels) as InvoiceStatus[];
