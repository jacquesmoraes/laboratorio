export interface SystemSettings {
  labName: string;
  email: string;
  phone: string;
  cnpj: string;
  footerMessage: string;
  logoUrl?: string;
  lastUpdated: Date;
  address: LabAddress;
}

export interface LabAddress {
  street: string;
  cep: string;
  number: number;
  complement: string;
  neighborhood: string;
  city: string;
}

export interface UpdateSystemSettingsDto {
  labName: string;
  email: string;
  phone: string;
  cnpj: string;
  footerMessage: string;
  labAddressRecord: LabAddress;
}