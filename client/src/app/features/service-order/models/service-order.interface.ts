export interface ServiceOrder {
  billingInvoiceId: any;
  serviceOrderId: number;
  orderNumber: string;
  dateIn: string;
  patientName: string;
  status: OrderStatus;
  clientName: string;
  clientId: number;
  orderTotal: number;
  currentSectorName?: string;
  lastMovementDate?: string;
}

export interface ServiceOrderDetails extends ServiceOrder {
  dateOut: string;
  dateOutFinal?: string;
  client: ClientInvoice;
  works: Work[];
  stages: Stage[];
}

export interface ServiceOrderAlert {
  serviceOrderId: number;
  orderNumber: string;
  patientName: string;
  clientName: string;
  currentSectorName: string;
  lastTryInDate: string;
  daysOut: number;
  status: string;
}

export interface ClientInvoice {
  clientId:number;
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

export interface Work {
  workTypeId: number;
  workTypeName: string;
  quantity: number;
  priceUnit: number;
  priceTotal: number;
  shadeColor?: string;
  scaleName?: string;
  notes?: string;
}

export interface Stage {
  sectorId: number;
  sectorName: string;
  dateIn: string;
  dateOut?: string;
}

export interface CreateServiceOrderDto {
  clientId: number;
  dateIn: string;
  patientName: string;
  firstSectorId: number;
  works: CreateWorkDto[];
}

export interface CreateWorkDto {
  workTypeId: number;
  quantity: number;
  priceUnit?: number;
  shadeId?: number;
  scaleId?: number;
  notes?: string;
}

export interface MoveToStageDto {
  serviceOrderId: number;
  sectorId: number;
  dateIn: string;
}

export interface SendToTryInDto {
  serviceOrderId: number;
  dateOut: string;
}

export interface FinishOrderDto {
  serviceOrderIds: number[];
  dateOut: string;
}

export interface ServiceOrderParams {
  pageNumber: number;
  pageSize: number;
  sort?: string;
  search?: string;
  excludeFinished?: boolean;
  clientId?: number;
  status?: OrderStatus;
  excludeInvoiced?: boolean;
  
}

export interface Pagination<T> {
  pageNumber: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
  data: T[];
}

export enum OrderStatus {
  Production = 1,
  TryIn = 2,
  Finished = 3
}

export const OrderStatusLabels: Record<OrderStatus, string> = {
  [OrderStatus.Production]: 'Em Produção',
  [OrderStatus.TryIn]: 'Em Prova',
  [OrderStatus.Finished]: 'Finalizado'
};