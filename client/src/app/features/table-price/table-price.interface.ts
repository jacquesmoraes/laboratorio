export interface TablePrice {
  id: number;
  name: string;
  description: string;
  status: boolean;
  items: TablePriceItem[];
  clients: ClientForTablePrice[];
}

export interface TablePriceItem {
  workTypeId: number;
  workTypeName: string;
  price: number;
}

export interface ClientForTablePrice {
  clientId: number;
  clientName: string;
  billingMode: number;
  tablePriceName: string;
}

export interface TablePriceOption {
  value: number;
  label: string;
  description?: string;
}