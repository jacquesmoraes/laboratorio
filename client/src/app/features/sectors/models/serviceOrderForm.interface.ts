
/**
 * Dados básicos carregados na primeira etapa do formulário
 */
export interface BasicFormData {
  clients: ClientFormData[];
  sectors: SectorFormData[];
}

/**
 * Dados de works carregados na segunda etapa do formulário
 */
export interface WorksFormData {
  workTypes: WorkTypeFormData[];
  scales: ScaleFormData[];
  shades: ShadeFormData[];
}

/**
 * Dados do cliente otimizados para o formulário
 */
export interface ClientFormData {
  clientId: number;
  clientName: string;
  clientPhoneNumber: string;
  city: string;
  isInactive: boolean;
  billingMode: number;
  tablePriceName: string;
  tablePriceId: number;
}

/**
 * Dados do setor otimizados para o formulário
 */
export interface SectorFormData {
  sectorId: number;
  name: string;
}

/**
 * Dados do tipo de trabalho otimizados para o formulário
 */
export interface WorkTypeFormData {
  id: number;
  name: string;
  description: string;
  isActive: boolean;
  workSectionId: number;
  workSectionName: string | null;
}

/**
 * Dados da escala otimizados para o formulário
 */
export interface ScaleFormData {
  id: number;
  name: string;
}

/**
 * Dados da cor/shade otimizados para o formulário
 */
export interface ShadeFormData {
  id: number;
  color: string;
  scaleId: number | null;
}