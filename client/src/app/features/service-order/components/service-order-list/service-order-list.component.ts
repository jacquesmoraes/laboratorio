import {
  Component,
  signal,
  computed,
  inject,
  OnInit,
  OnDestroy,
  ChangeDetectionStrategy,
  effect,
} from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { FormsModule } from '@angular/forms';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';

import { OrderStatus, ServiceOrderParams } from '../../models/service-order.interface';
import { ServiceOrderListService } from './services/service-order-list.service';
import { PageEvent } from '@angular/material/paginator';
import { SERVICE_ORDER_LIST_CONFIG } from './models/service-order-list.config';
import { ServiceOrderTableComponent, TableActionEvent } from './service-order-table/service-order-table.component';
import { ScheduleDeliveryModalComponent } from '../schedule-delivery-modal/schedule-delivery-modal.component';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-service-order-list',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    FormsModule,
    ServiceOrderTableComponent,
  ],
  templateUrl: './service-order-list.component.html',
  styleUrls: ['./service-order-list.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ServiceOrderListComponent implements OnInit, OnDestroy {
  private readonly serviceOrderListService = inject(ServiceOrderListService);
  private readonly router = inject(Router);
  private readonly dialog = inject(MatDialog);
  private readonly snackBar = inject(MatSnackBar); 
  private readonly destroy$ = new Subject<void>();

  // Estado do componente
  readonly serviceOrders = this.serviceOrderListService.serviceOrders;
  readonly selectedOrderIds = this.serviceOrderListService.selectedOrderIds;
  readonly pagination = this.serviceOrderListService.pagination;
  readonly loading = this.serviceOrderListService.loading;
  readonly sectors = this.serviceOrderListService.sectors;
  readonly currentParams = this.serviceOrderListService.currentParams;

  // Computed values
  readonly totalPages = this.serviceOrderListService.totalPages;
  readonly totalItems = this.serviceOrderListService.totalItems;
  readonly hasSelection = this.serviceOrderListService.hasSelection;

  // Configurações
  readonly pageSizeOptions = SERVICE_ORDER_LIST_CONFIG.pageSizeOptions;

  // Computed para paginação
  readonly pageSize = computed(() => this.currentParams().pageSize);
  readonly pageIndex = computed(() => this.currentParams().pageNumber - 1);

  // Filtros inline
  searchFilter = signal('');
  statusFilter = signal<OrderStatus | ''>('');
  sortFilter = signal('DateIn');
  showFinishedOrders = signal(false);

  ngOnInit() {
    
  this.setupSearchDebounce();
  this.loadServiceOrders();
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
    this.serviceOrderListService.destroy();
  }

  // Configuração do debounce para busca
  private setupSearchDebounce() {
    // Criar um observable que emite quando o searchFilter muda
    // Como estamos usando signals, vamos usar um approach diferente
    // Vamos implementar o debounce diretamente no método onSearchChange
  }

  // Carregamento de dados
  loadServiceOrders() {
    this.serviceOrderListService.loadServiceOrders().subscribe();
  }

  // Eventos da tabela
  onSelectionChange(orderId: number) {
    this.serviceOrderListService.toggleSelection(orderId);
  }

  onPageChange(event: PageEvent) {
    this.serviceOrderListService.updateParams({
      pageNumber: event.pageIndex + 1,
      pageSize: event.pageSize,
    });
    this.loadServiceOrders();
  }

  onAction(event: TableActionEvent) {
    switch (event.type) {
      case 'view':
        this.viewDetails(event.orderId);
        break;
      case 'edit':
        this.editOrder(event.orderId);
        break;
      case 'sendToTryIn':
        this.sendToTryIn(event.orderId);
        break;
      case 'moveToStage':
        this.moveToStage(event.orderId);
        break;
      case 'reopen':
        this.reopenOrder(event.orderId);
        break;
    }
  }

  // Navegação
  navigateToNew() {
    this.router.navigate(['/admin/service-orders/new']);
  }

  viewDetails(id: number) {
    this.router.navigate(['/admin/service-orders', id]);
  }

  editOrder(id: number) {
    this.router.navigate(['/admin/service-orders', id, 'edit']);
  }

  // Ações de OS
  sendToTryIn(orderId: number) {
    this.serviceOrderListService.sendToTryIn(orderId).subscribe();
  }

  moveToStage(orderId: number) {
    this.serviceOrderListService.moveToStage(orderId).subscribe({
      next: () => {
         this.openScheduleModal(orderId);
      }
    });
  }

  private openScheduleModal(serviceOrderId: number) {
    const scheduleDialogRef = this.dialog.open(ScheduleDeliveryModalComponent, {
      width: '400px',
      data: {
        serviceOrderId: serviceOrderId,
        sectors: this.sectors().map(s => ({ 
          sectorId: s.id, 
          sectorName: s.name 
        }))
      }
    });

    scheduleDialogRef.afterClosed().subscribe((scheduleSuccess) => {
      if (scheduleSuccess) {
        this.snackBar.open('Entrega agendada com sucesso!', 'Fechar', {
          duration: 3000,
          panelClass: ['success-snackbar']
        });
      }
    });
  }

  reopenOrder(orderId: number) {
    this.serviceOrderListService.reopenOrder(orderId).subscribe();
  }

  finishSelectedOrders() {
    this.serviceOrderListService.finishSelectedOrders().subscribe();
  }


  // Métodos para os filtros inline com debounce
  private searchDebounceTimeout?: number;

  onSearchChange(value: string) {
    this.searchFilter.set(value);
    
    // Cancelar timeout anterior
    if (this.searchDebounceTimeout) {
      clearTimeout(this.searchDebounceTimeout);
    }
    
    // Criar novo timeout para debounce
    this.searchDebounceTimeout = window.setTimeout(() => {
      this.updateFilters();
    }, SERVICE_ORDER_LIST_CONFIG.searchDebounceTime);
  }

  onStatusChange(value: OrderStatus | '') {
    this.statusFilter.set(value);
    this.updateFilters();
  }

  onSortChange(value: string) {
    this.sortFilter.set(value);
    this.updateFilters();
  }

  onShowFinishedOrdersChange(value: boolean) {
    this.showFinishedOrders.set(value);
    this.updateFilters();
  }

private readonly syncFiltersEffect = effect(() => {
  const p = this.currentParams();
  this.searchFilter.set(p.search ?? '');
  this.statusFilter.set((p.status ?? '') as any);
  this.sortFilter.set(p.sort ?? 'DateIn');
  this.showFinishedOrders.set(!p.excludeFinished);
});


  clearFilters() {
    this.searchFilter.set('');
    this.statusFilter.set('');
    this.sortFilter.set('DateIn');
    this.showFinishedOrders.set(false);
    
    // Cancelar timeout de busca se existir
    if (this.searchDebounceTimeout) {
      clearTimeout(this.searchDebounceTimeout);
    }
    
    this.updateFilters();
  }

  private updateFilters() {
    this.serviceOrderListService.updateParams({
      search: this.searchFilter(),
      status: this.statusFilter() || undefined,
      sort: this.sortFilter(),
      excludeFinished: !this.showFinishedOrders(),
      pageNumber: 1, // Reset para primeira página
    });
    this.loadServiceOrders();
  }
}