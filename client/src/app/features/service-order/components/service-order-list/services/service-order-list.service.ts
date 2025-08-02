import { Injectable, inject, signal, computed } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatDialog } from '@angular/material/dialog';
import { Observable, Subject } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';

import { 
  ServiceOrder, 
  ServiceOrderParams, 
  MoveToStageDto, 
  SendToTryInDto, 
  FinishOrderDto,
  Pagination 
} from '../../../models/service-order.interface';
import { ServiceOrdersService } from '../../../services/service-order.service';
import { SectorService } from '../../../../sectors/service/sector.service';
import { Sector } from '../../../../sectors/models/sector.interface';
import { SERVICE_ORDER_MESSAGES, LOADING_CONFIG } from '../models/service-order-list.config';
import { MoveToStageDialogComponent } from '../dialogs/move-to-stage-dialog.component';
import { FinishOrdersDialogComponent } from '../dialogs/finish-orders-dialog.component';
import { ErrorMappingService } from '../../../../../core/services/error.mapping.service';


export interface ServiceOrderListState {
  serviceOrders: ServiceOrder[];
  selectedOrderIds: number[];
  pagination: Pagination<ServiceOrder> | null;
  loading: boolean;
  sectors: Sector[];
  currentParams: ServiceOrderParams;
}

@Injectable({
  providedIn: 'root'
})
export class ServiceOrderListService {
  private readonly serviceOrdersService = inject(ServiceOrdersService);
  private readonly sectorService = inject(SectorService);
  private readonly errorMappingService = inject(ErrorMappingService);
  private readonly snackBar = inject(MatSnackBar);
  private readonly dialog = inject(MatDialog);
  private readonly destroy$ = new Subject<void>();

  // Estado usando signals
  private readonly state = signal<ServiceOrderListState>({
    serviceOrders: [],
    selectedOrderIds: [],
    pagination: null,
    loading: false,
    sectors: [],
    currentParams: {
      pageNumber: 1,
      pageSize: 10,
      sort: 'DateIn',
      search: '',
      excludeFinished: true,
    }
  });

  // Getters para o estado
  readonly serviceOrders = computed(() => this.state().serviceOrders);
  readonly selectedOrderIds = computed(() => this.state().selectedOrderIds);
  readonly pagination = computed(() => this.state().pagination);
  readonly loading = computed(() => this.state().loading);
  readonly sectors = computed(() => this.state().sectors);
  readonly currentParams = computed(() => this.state().currentParams);

  // Computed values
  readonly totalPages = computed(() => this.pagination()?.totalPages || 1);
  readonly totalItems = computed(() => this.pagination()?.totalItems || 0);
  readonly hasSelection = computed(() => this.selectedOrderIds().length > 0);

  constructor() {
    this.loadSectors();
  }
  

  // Métodos para atualizar estado
  updateParams(params: Partial<ServiceOrderParams>) {
    this.state.update(state => ({
      ...state,
      currentParams: { ...state.currentParams, ...params }
    }));
  }

  setLoading(loading: boolean) {
    this.state.update(state => ({ ...state, loading }));
  }

  setSelectedOrderIds(ids: number[]) {
    this.state.update(state => ({ ...state, selectedOrderIds: ids }));
  }

  toggleSelection(id: number) {
    const current = this.selectedOrderIds();
    const newSelection = current.includes(id) 
      ? current.filter(x => x !== id)
      : [...current, id];
    
    this.setSelectedOrderIds(newSelection);
  }

  // Carregamento de dados
    loadServiceOrders(): Observable<Pagination<ServiceOrder>> {
    this.setLoading(true);
    
    return new Observable(observer => {
      // ✅ USAR: Método otimizado que usa PagedLightForLists no backend
      this.serviceOrdersService.getServiceOrders(this.currentParams()).subscribe({
        next: (res) => {
          this.state.update(state => ({
            ...state,
            serviceOrders: res.data,
            pagination: res,
            selectedOrderIds: [],
            loading: false
          }));
          observer.next(res);
          observer.complete();
        },
        error: (err: HttpErrorResponse) => {
          console.error('Error loading service orders:', err);
          this.setLoading(false);
          this.showError(SERVICE_ORDER_MESSAGES.error.loadOrders);
          observer.error(err);
        }
      });
    });
  }



  // Ações de OS
 
  sendToTryIn(orderId: number): Observable<void> {
    const today = new Date().toISOString().split('T')[0];
    const sendToTryInDto: SendToTryInDto = {
      serviceOrderId: orderId,
      dateOut: today,
    };

    this.setLoading(true);
    
    return new Observable(observer => {
      this.serviceOrdersService.sendToTryIn(sendToTryInDto).subscribe({
        next: () => {
          this.showSuccess(SERVICE_ORDER_MESSAGES.success.sentToTryIn);
          this.loadServiceOrders().subscribe();
          this.setLoading(false);
          observer.next();
          observer.complete();
        },
        error: (err: HttpErrorResponse) => {
          console.error('Error sending to try in:', err);
          const errorMessage = this.extractErrorMessage(err);
          this.showError(errorMessage);
          this.setLoading(false);
          observer.error(err);
        },
      });
    });
  }

  moveToStage(orderId: number): Observable<void> {
    const dialogRef = this.dialog.open(MoveToStageDialogComponent, {
      width: '400px',
      data: {
        sectors: this.sectors(),
        orderId: orderId,
      },
    });

    return new Observable(observer => {
      dialogRef.afterClosed().subscribe((result) => {
        if (result) {
          const moveToStageDto: MoveToStageDto = {
            serviceOrderId: orderId,
            sectorId: result.sectorId,
            dateIn: result.dateIn,
          };

          this.setLoading(true);
          this.serviceOrdersService.moveToStage(moveToStageDto).subscribe({
            next: () => {
              this.showSuccess(SERVICE_ORDER_MESSAGES.success.movedToStage);
              this.loadServiceOrders().subscribe();
              this.setLoading(false);
              observer.next();
              observer.complete();
            },
            error: (err: HttpErrorResponse) => {
              console.error('Error moving to stage:', err);
              const errorMessage = this.extractErrorMessage(err);
              this.showError(errorMessage);
              this.setLoading(false);
              observer.error(err);
            },
          });
        } else {
          observer.complete();
        }
      });
    });
  }

  finishSelectedOrders(): Observable<void> {
    const selectedIds = this.selectedOrderIds();
    if (selectedIds.length === 0) {
      this.showError(SERVICE_ORDER_MESSAGES.validation.selectOrders);
      return new Observable(observer => observer.complete());
    }

    // Busca as OS selecionadas
    const selectedOrders = this.serviceOrders().filter(order => selectedIds.includes(order.serviceOrderId));
    
    // Garante que todas são do mesmo cliente
    const clientIds = [...new Set(selectedOrders.map(order => order.clientId))];
    if (clientIds.length > 1) {
      this.showError(SERVICE_ORDER_MESSAGES.validation.sameClient);
      return new Observable(observer => observer.complete());
    }

    // Abrir diálogo de confirmação
    const dialogRef = this.dialog.open(FinishOrdersDialogComponent, {
      width: '500px',
      data: {
        orderCount: selectedIds.length,
        clientName: selectedOrders[0].clientName,
      },
    });

    return new Observable(observer => {
      dialogRef.afterClosed().subscribe((result) => {
        if (result) {
          const finishOrderDto: FinishOrderDto = {
            serviceOrderIds: selectedIds,
            dateOut: result.dateOut,
          };

          this.setLoading(true);
          this.serviceOrdersService.finishOrders(finishOrderDto).subscribe({
            next: () => {
              this.showSuccess(SERVICE_ORDER_MESSAGES.success.finished(selectedIds.length));
              this.loadServiceOrders().subscribe();
              this.setLoading(false);
              observer.next();
              observer.complete();
            },
            error: (err: HttpErrorResponse) => {
              console.error('Error finishing orders:', err);
              const errorMessage = this.extractErrorMessage(err);
              this.showError(errorMessage);
              this.setLoading(false);
              observer.error(err);
            },
          });
        } else {
          observer.complete();
        }
      });
    });
  }

  reopenOrder(orderId: number): Observable<void> {
    this.setLoading(true);
    
    return new Observable(observer => {
      this.serviceOrdersService.reopenServiceOrder(orderId).subscribe({
        next: () => {
          this.showSuccess(SERVICE_ORDER_MESSAGES.success.reopened);
          this.loadServiceOrders().subscribe();
          this.setLoading(false);
          observer.next();
          observer.complete();
        },
        error: (err: HttpErrorResponse) => {
          console.error('Error reopening order:', err);
          const errorMessage = this.extractErrorMessage(err);
          this.showError(errorMessage);
          this.setLoading(false);
          observer.error(err);
        },
      });
    });
  }
  



  // Métodos auxiliares


private loadSectors() {
    // ✅ USAR: SectorService que já usa specifications otimizadas
    this.sectorService.getAll().subscribe({
      next: (sectors) => {
        this.state.update(state => ({ ...state, sectors }));
      },
      error: (err: HttpErrorResponse) => {
        console.error('Error loading sectors:', err);
        this.showError(SERVICE_ORDER_MESSAGES.error.loadSectors);
      },
    });
  }
  
  private showSuccess(message: string) {
    this.snackBar.open(message, SERVICE_ORDER_MESSAGES.actions.close, {
      duration: LOADING_CONFIG.snackBarDuration,
    });
  }

  private showError(message: string) {
    this.snackBar.open(message, SERVICE_ORDER_MESSAGES.actions.close, {
      duration: LOADING_CONFIG.errorSnackBarDuration,
    });
  }

  private extractErrorMessage(error: HttpErrorResponse | string): string {
    return this.errorMappingService.mapServiceOrderError(error);
  }

  // Cleanup
  destroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }
}