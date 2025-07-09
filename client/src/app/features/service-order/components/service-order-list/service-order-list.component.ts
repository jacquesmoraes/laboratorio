import {
  Component,
  signal,
  computed,
  inject,
  OnInit,
  OnDestroy,
  ChangeDetectionStrategy,
} from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

import {  ServiceOrderParams } from '../../models/service-order.interface';
import { ServiceOrderListService } from './services/service-order-list.service';
import { ServiceOrderFiltersComponent } from './service-order-filters/service-order-filters.component';

import { FilterChangeEvent } from './service-order-filters/service-order-filters.component';
import { PageEvent } from '@angular/material/paginator';
import { SERVICE_ORDER_LIST_CONFIG } from './models/service-order-list.config';
import { ServiceOrderTableComponent, TableActionEvent } from './service-order-table/service-order-table.component';

@Component({
  selector: 'app-service-order-list',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    ServiceOrderFiltersComponent,
    ServiceOrderTableComponent,
  ],
  templateUrl: './service-order-list.component.html',
  styleUrls: ['./service-order-list.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ServiceOrderListComponent implements OnInit, OnDestroy {
  private readonly serviceOrderListService = inject(ServiceOrderListService);
  private readonly router = inject(Router);

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

  ngOnInit() {
    this.loadServiceOrders();
  }

  ngOnDestroy() {
    this.serviceOrderListService.destroy();
  }

  // Carregamento de dados
  loadServiceOrders() {
    this.serviceOrderListService.loadServiceOrders().subscribe();
  }

  // Eventos de filtros
  onFilterChange(event: FilterChangeEvent) {
    const newParams: Partial<ServiceOrderParams> = {
      search: event.search,
      status: event.status || undefined,
      sort: event.sortBy,
      excludeFinished: !event.showFinishedOrders,
      pageNumber: 1, // Reset para primeira página
    };

    this.serviceOrderListService.updateParams(newParams);
    this.loadServiceOrders();
  }

  // Eventos da tabela
  onSelectionChange(orderId: number) {
    this.serviceOrderListService.toggleSelection(orderId);
  }

  onPageChange(event: PageEvent) {
    const newParams: Partial<ServiceOrderParams> = {
      pageNumber: event.pageIndex + 1,
      pageSize: event.pageSize,
    };

    this.serviceOrderListService.updateParams(newParams);
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
    this.router.navigate(['/service-orders/new']);
  }

  viewDetails(id: number) {
    this.router.navigate(['/service-orders', id]);
  }

  editOrder(id: number) {
    this.router.navigate(['/service-orders', id, 'edit']);
  }

  // Ações de OS
  sendToTryIn(orderId: number) {
    this.serviceOrderListService.sendToTryIn(orderId).subscribe();
  }

  moveToStage(orderId: number) {
    this.serviceOrderListService.moveToStage(orderId).subscribe();
  }

  reopenOrder(orderId: number) {
    this.serviceOrderListService.reopenOrder(orderId).subscribe();
  }

  finishSelectedOrders() {
    this.serviceOrderListService.finishSelectedOrders().subscribe();
  }
}