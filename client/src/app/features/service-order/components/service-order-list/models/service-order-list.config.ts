import { OrderStatus, OrderStatusLabels } from '../../../models/service-order.interface';

export interface ServiceOrderListConfig {
  readonly displayedColumns: string[];
  readonly statusMap: Record<string, OrderStatus>;
  readonly orderStatuses: Array<{ value: OrderStatus; label: string }>;
  readonly statusClasses: Record<number, string>;
  readonly sortOptions: Array<{ value: string; label: string }>;
  readonly pageSizeOptions: number[];
  readonly defaultPageSize: number;
  readonly defaultSort: string;
  readonly searchDebounceTime: number;
}

export const SERVICE_ORDER_LIST_CONFIG: ServiceOrderListConfig = {
  displayedColumns: [
    'select',
    'orderNumber',
    'clientName',
    'patientName',
    'dateIn',
    'lastMovementDate',
    'status',
    'currentSectorName',
    'actions',
  ],

  statusMap: {
    Production: OrderStatus.Production,
    TryIn: OrderStatus.TryIn,
    Finished: OrderStatus.Finished,
  },

  orderStatuses: [
    { value: OrderStatus.Production, label: OrderStatusLabels[OrderStatus.Production] },
    { value: OrderStatus.TryIn, label: OrderStatusLabels[OrderStatus.TryIn] },
    { value: OrderStatus.Finished, label: OrderStatusLabels[OrderStatus.Finished] },
  ],

  statusClasses: {
    [OrderStatus.Production]: 'status-production',
    [OrderStatus.TryIn]: 'status-tryin',
    [OrderStatus.Finished]: 'status-finished',
  },

  sortOptions: [
    { value: 'DateIn', label: 'Data de Entrada' },
    { value: 'PatientName', label: 'Nome do Paciente' },
    { value: 'OrderNumber', label: 'Número da Ordem' },
  ],

  pageSizeOptions: [5, 10, 25, 50],
  defaultPageSize: 10,
  defaultSort: 'DateIn',
  searchDebounceTime: 300,
};

// Tipos para os dados dos diálogos
export interface MoveToStageDialogData {
  sectors: Array<{ id: number; name: string }>;
  orderId: number;
}

export interface FinishOrdersDialogData {
  orderCount: number;
  clientName: string;
}

// Constantes para mensagens
export const SERVICE_ORDER_MESSAGES = {
  success: {
    reopened: 'Ordem reaberta com sucesso!',
    movedToStage: 'Ordem movida para o setor com sucesso!',
    sentToTryIn: 'Ordem enviada para prova com sucesso!',
    finished: (count: number) => `${count} ordem(ns) finalizada(s) com sucesso!`,
  },
  error: {
    reopen: 'Erro ao reabrir ordem. Tente novamente.',
    moveToStage: 'Erro ao mover para o setor. Tente novamente.',
    sendToTryIn: 'Erro ao enviar para prova. Tente novamente.',
    finish: 'Erro ao finalizar ordens. Tente novamente.',
    loadOrders: 'Erro ao carregar ordens de serviço.',
    loadSectors: 'Erro ao carregar setores.',
  },
  validation: {
    selectOrders: 'Selecione pelo menos uma ordem de serviço para finalizar.',
    sameClient: 'Todas as ordens de serviço selecionadas devem ser do mesmo cliente.',
    noOpenStage: 'Não há estágio aberto para enviar para prova.',
    invalidDate: 'Data inválida. Verifique a data informada.',
    orderFinished: 'Não é possível alterar uma ordem finalizada.',
    orderInTryIn: 'A ordem já está em prova.',
    orderNotFound: 'Ordem de serviço não encontrada.',
  },
  actions: {
    close: 'Fechar',
    cancel: 'Cancelar',
    confirm: 'Confirmar',
    finish: 'Finalizar',
  },
} as const;

// Configurações de loading
export const LOADING_CONFIG = {
  snackBarDuration: 3000,
  errorSnackBarDuration: 4000,
} as const;