import type { WorkType } from "../works/models/work-type.interface";


// Main interface for table price
export interface TablePrice {
  id: number;
  name: string;
  description: string;
  status: boolean;
  items: TablePriceItem[];
  clients: ClientForTablePrice[];
}

// Interface for table price items
export interface TablePriceItem {
  workTypeId: number;
  workTypeName: string;
  price: number;
}

// Interface for clients associated with table price
export interface ClientForTablePrice {
  clientId: number;
  clientName: string;
  billingMode: number;
  tablePriceName: string;
}

// Interface for dropdown options
export interface TablePriceOption {
  value: number;
  label: string;
  description?: string;
}

// DTOs for API operations
export interface CreateTablePriceDto {
  name: string;
  description: string;
  items: TablePriceItemInputDto[];
}

export interface UpdateTablePriceDto {
  id: number;
  name: string;
  description: string;
  status: boolean;
  items: TablePriceItemInputDto[];
}

export interface TablePriceItemInputDto {
  workTypeId: number;
  price: number;
}
export interface ClientWorkPrice {
  workTypeId: number;
  workTypeName: string;
  price: number;
}

// Re-export WorkType for convenience
export { WorkType };

// Additional utility types
export type BillingMode = 1 | 2; // 1 = Por Ordem, 2 = Por Item

export interface TablePriceFormData {
  name: string;
  description: string;
  status: boolean;
  items: TablePriceItemFormData[];
}

export interface TablePriceItemFormData {
  workTypeId: number;
  price: number;
}