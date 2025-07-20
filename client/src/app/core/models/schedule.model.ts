export interface ScheduleDeliveryDto {
  serviceOrderId: number;
  scheduledDate: string; // ISO 8601
  deliveryType: ScheduledDeliveryType;
  sectorId?: number;
}

export type ScheduledDeliveryType =
  | 'SectorTransfer'
  | 'TryIn'
  | 'FinalDelivery';

export interface ScheduleItemRecord {
  scheduleId: number;
  serviceOrderId: number;
  orderNumber: string;
  patientName: string;
  clientName: string;
  scheduledDate: string;
  deliveryType: ScheduledDeliveryType;
  isDelivered: boolean;
  isOverdue: boolean;
  status: string;
  currentSectorName?: string;
  targetSectorName?: string;
}

export interface SectorScheduleRecord {
  sectorId: number;
  sectorName: string;
  deliveries: ScheduleItemRecord[];
}
