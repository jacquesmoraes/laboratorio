import { Component, signal, inject, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ClientAreaService } from '../../services/client-area.service';
import { DataTableComponent } from '../shared/data-table.component';
import { ClientPayment, PaymentParams, PaginatedResponse, TableColumn } from '../../models/client-area.interface';

@Component({
  selector: 'app-client-area-payments',
  standalone: true,
  imports: [CommonModule, DataTableComponent],
  template: `
    <div class="payments-page">
      <div class="page-header">
        <h2>Pagamentos</h2>
        <p>Histórico de seus pagamentos</p>
      </div>

      <app-data-table
        [columns]="columns()"
        [data]="payments()"
        [paginationInfo]="paginationInfo()"
        [showFilters]="true"
        [showPagination]="true"
        [showStatusFilter]="false"
        [showDateFilters]="true"
        (pageChange)="onPageChange($event)"
        (searchChange)="onSearchChange($event)"
        (startDateChange)="onStartDateChange($event)"
        (endDateChange)="onEndDateChange($event)"
        (sortChange)="onSortChange($event)"
      />
    </div>
  `,
  styles: [`
    .payments-page {
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
export class ClientAreaPaymentsComponent {
  private readonly clientAreaService = inject(ClientAreaService);

  payments = signal<ClientPayment[]>([]);
  paginationInfo = signal<any>(null);
  loading = signal(false);

  columns = signal<TableColumn[]>([
    { key: 'paymentDate', label: 'Data', sortable: true },
    { key: 'amountPaid', label: 'Valor', sortable: true },
    { key: 'description', label: 'Descrição', sortable: false },
    { key: 'invoiceNumber', label: 'Fatura', sortable: true }
  ]);

  private currentParams = signal<PaymentParams>({
    pageNumber: 1,
    pageSize: 10
  });

  constructor() {
    this.loadPayments();
  }

  private loadPayments() {
    this.loading.set(true);
    
    this.clientAreaService.getPayments(this.currentParams()).subscribe({
      next: (response: PaginatedResponse<ClientPayment>) => {
        this.payments.set(response.data);
        this.paginationInfo.set({
          pageNumber: response.pageNumber,
          pageSize: response.pageSize,
          totalPages: response.totalPages,
          totalItems: response.totalItems
        });
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Erro ao carregar pagamentos', err);
        this.loading.set(false);
      }
    });
  }

  onPageChange(page: number) {
    this.currentParams.update(params => ({ ...params, pageNumber: page }));
    this.loadPayments();
  }

  onSearchChange(search: string) {
    this.currentParams.update(params => ({ ...params, search, pageNumber: 1 }));
    this.loadPayments();
  }

  onStartDateChange(date: string) {
    this.currentParams.update(params => ({ 
      ...params, 
      startDate: date || undefined, 
      pageNumber: 1 
    }));
    this.loadPayments();
  }

  onEndDateChange(date: string) {
    this.currentParams.update(params => ({ 
      ...params, 
      endDate: date || undefined, 
      pageNumber: 1 
    }));
    this.loadPayments();
  }

  onSortChange(sort: {column: string, direction: string}) {
    // Implementar sorting se necessário
    console.log('Sort:', sort);
  }
}