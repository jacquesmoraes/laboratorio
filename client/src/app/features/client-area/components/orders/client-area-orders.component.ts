import { Component, signal, inject, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ClientAreaService } from '../../services/client-area.service';
import { DataTableComponent } from '../shared/data-table.component';
import { ServiceOrder, OrderParams, PaginatedResponse, TableColumn } from '../../models/client-area.interface';

@Component({
  selector: 'app-client-area-orders',
  standalone: true,
  imports: [CommonModule, DataTableComponent],
  template: `
    <div class="orders-page">
      <div class="page-header">
        <h2>Ordens de Serviço</h2>
        <p>Acompanhe o status de suas ordens de serviço</p>
      </div>

      <app-data-table
        [columns]="columns()"
        [data]="orders()"
        [paginationInfo]="paginationInfo()"
        [showFilters]="true"
        [showPagination]="true"
        [showStatusFilter]="true"
        [showDateFilters]="false"
        (pageChange)="onPageChange($event)"
        (searchChange)="onSearchChange($event)"
        (statusChange)="onStatusChange($event)"
        (sortChange)="onSortChange($event)"
      />
    </div>
  `,
  styles: [`
    .orders-page {
      display: flex;
      flex-direction: column;
      gap: 1.5rem;
    }

    .page-header {
      background: white;
      padding: 1.5rem;
      border-radius: 8px;
      box-shadow: 0 2px 4px rgba(0,0,0,0.1);
    }

    .page-header h2 {
      margin: 0 0 0.5rem 0;
      color: #276678;
    }

    .page-header p {
      margin: 0;
      color: #6c757d;
    }
  `],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ClientAreaOrdersComponent {
  private readonly clientAreaService = inject(ClientAreaService);

  orders = signal<ServiceOrder[]>([]);
  paginationInfo = signal<any>(null);
  loading = signal(false);

  columns = signal<TableColumn[]>([
    { key: 'orderNumber', label: 'Número', sortable: true },
    { key: 'patientName', label: 'Paciente', sortable: true },
    { key: 'dateIn', label: 'Data de Entrada', sortable: true },
    { key: 'lastMovementDate', label: 'Último Movimento', sortable: true },
    { key: 'currentSectorName', label: 'Setor Atual', sortable: true },
    { key: 'status', label: 'Status', sortable: true },
    { key: 'totalAmount', label: 'Valor', sortable: true }
  ]);

  private currentParams = signal<OrderParams>({
    pageNumber: 1,
    pageSize: 10
  });

  constructor() {
    this.loadOrders();
  }

  private loadOrders() {
    this.loading.set(true);
    
    this.clientAreaService.getOrders(this.currentParams()).subscribe({
      next: (response: PaginatedResponse<ServiceOrder>) => {
        this.orders.set(response.data);
        this.paginationInfo.set({
          pageNumber: response.pageNumber,
          pageSize: response.pageSize,
          totalPages: response.totalPages,
          totalItems: response.totalItems
        });
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Erro ao carregar ordens', err);
        this.loading.set(false);
      }
    });
  }

  onPageChange(page: number) {
    this.currentParams.update(params => ({ ...params, pageNumber: page }));
    this.loadOrders();
  }

  onSearchChange(search: string) {
    this.currentParams.update(params => ({ ...params, search, pageNumber: 1 }));
    this.loadOrders();
  }

  onStatusChange(status: string) {
    this.currentParams.update(params => ({ 
      ...params, 
      status: status || undefined, 
      pageNumber: 1 
    }));
    this.loadOrders();
  }

  onSortChange(sort: {column: string, direction: string}) {
    // Implementar sorting se necessário
    console.log('Sort:', sort);
  }
}