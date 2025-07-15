import { Component, signal, inject, ChangeDetectionStrategy, TemplateRef } from '@angular/core';
import { CommonModule, NgIf } from '@angular/common';

import { ClientAreaService } from '../../services/client-area.service';
import { DataTableComponent } from '../shared/data-table.component';
import { ActionTemplateContext, ClientInvoice, InvoiceParams, PaginatedResponse, TableAction, TableColumn } from '../../models/client-area.interface';

@Component({
  selector: 'app-client-area-invoices',
  standalone: true,
  imports: [CommonModule, DataTableComponent],
  template: `
    <div class="invoices-page">
      <div class="page-header">
        <h2>Faturas</h2>
        <p>Visualize e gerencie suas faturas</p>
      </div>

      @if (loading()) {
        <div class="loading-container">
          <div class="loading-spinner"></div>
          <p>Carregando faturas...</p>
        </div>
      } @else {
         <app-data-table
          [columns]="columns()"
          [data]="invoices()"
          [paginationInfo]="paginationInfo()"
          [showFilters]="true"
          [showPagination]="true"
          [showStatusFilter]="true"
          [showDateFilters]="true"
          [statusOptions]="invoiceStatusOptions()"
          (pageChange)="onPageChange($event)"
          (searchChange)="onSearchChange($event)"
          (statusChange)="onStatusChange($event)"
          (startDateChange)="onStartDateChange($event)"
          (endDateChange)="onEndDateChange($event)"
          (sortChange)="onSortChange($event)"
          [actionTemplate]="actionTemplate"
          (actionClick)="onActionClick($event)"
        />
      }
<ng-template #actionTemplate let-item>
  <button 
    (click)="downloadInvoice(item)"
    [disabled]="downloadingInvoice() === item.billingInvoiceId"
    class="download-btn"
    [title]="downloadingInvoice() === item.billingInvoiceId ? 'Baixando...' : 'Baixar fatura'"
  >
     Download
  </button>
</ng-template>

      @if (error()) {
        <div class="error-container">
          <p>Erro ao carregar faturas: {{ error() }}</p>
          <button (click)="loadInvoices()" class="retry-btn">
            Tentar Novamente
          </button>
        </div>
      }
    </div>
  `,
  styles: [`
    .invoices-page {
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

    .loading-container {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      padding: 3rem;
      background: white;
      border-radius: 8px;
      box-shadow: 0 2px 4px rgba(0,0,0,0.1);
    }

    .loading-spinner {
      width: 40px;
      height: 40px;
      border: 4px solid #f3f3f3;
      border-top: 4px solid #276678;
      border-radius: 50%;
      animation: spin 1s linear infinite;
      margin-bottom: 1rem;
    }

    @keyframes spin {
      0% { transform: rotate(0deg); }
      100% { transform: rotate(360deg); }
    }

    .error-container {
      background: #f8d7da;
      color: #721c24;
      padding: 1rem;
      border-radius: 8px;
      border: 1px solid #f5c6cb;
      text-align: center;
    }

    .retry-btn {
      background: #276678;
      color: white;
      border: none;
      padding: 0.5rem 1rem;
      border-radius: 4px;
      cursor: pointer;
      margin-top: 0.5rem;
    }

    .retry-btn:hover {
      background: #1e4f5a;
    }
    .download-btn {
  background: #276678;
  color: white;
  border: none;
  padding: 0.5rem 1rem;
  border-radius: 4px;
  cursor: pointer;
  font-size: 0.9rem;
  transition: all 0.2s;
}

.download-btn:hover:not(:disabled) {
  background: #1e4f5a;
}

.download-btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}
  `],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ClientAreaInvoicesComponent {
  private readonly clientAreaService = inject(ClientAreaService);
downloadingInvoice = signal<number | null>(null);
  invoices = signal<ClientInvoice[]>([]);
  paginationInfo = signal<any>(null);
  loading = signal(false);
  error = signal<string | null>(null);
invoiceStatusOptions = signal([
    { value: '1', label: 'Parcialmente Pago' },
    { value: '2', label: 'Pago' },
    { value: '3', label: 'Cancelado' },
    { value: '4', label: 'Fechado' }
  ]);
  columns = signal<TableColumn[]>([
    { key: 'invoiceNumber', label: 'Número', sortable: true },
    { key: 'createdAt', label: 'Data', sortable: true },
    { key: 'totalServiceOrdersAmount', label: 'Valor Total', sortable: true },
    { key: 'totalPaid', label: 'Pago', sortable: true },
    { key: 'outstandingBalance', label: 'Saldo', sortable: true },
    { key: 'status', label: 'Status', sortable: true },
    { key: 'actions', label: 'Ações' }
  ]);

  private currentParams = signal<InvoiceParams>({
    pageNumber: 1,
    pageSize: 10
  });

  constructor() {
    this.loadInvoices();
  }
actionTemplate = this.createActionTemplate();

private createActionTemplate(): TemplateRef<ActionTemplateContext<ClientInvoice>> {
  // Template será definido no template do componente
  return {} as TemplateRef<ActionTemplateContext<ClientInvoice>>;
}

  loadInvoices() {
    this.loading.set(true);
    this.error.set(null);
    
    this.clientAreaService.getInvoices(this.currentParams()).subscribe({
      next: (response: PaginatedResponse<ClientInvoice>) => {
        console.log('✅ Faturas carregadas:', response);
        this.invoices.set(response.data);
        this.paginationInfo.set({
          pageNumber: response.pageNumber,
          pageSize: response.pageSize,
          totalPages: response.totalPages,
          totalItems: response.totalItems
        });
        this.loading.set(false);
      },
      error: (err) => {
        console.error('❌ Erro ao carregar faturas:', err);
        this.error.set(err.message || 'Erro desconhecido');
        this.loading.set(false);
      }
    });
  }

  onPageChange(page: number) {
    console.log('📄 Mudando para página:', page);
    this.currentParams.update(params => ({ ...params, pageNumber: page }));
    this.loadInvoices();
  }

  onSearchChange(search: string) {
    console.log('�� Buscando:', search);
    this.currentParams.update(params => ({ ...params, search, pageNumber: 1 }));
    this.loadInvoices();
  }

  onStatusChange(status: string) {
    console.log('🏷️ Filtrando por status:', status);
    this.currentParams.update(params => ({ 
      ...params, 
      status: status || undefined, 
      pageNumber: 1 
    }));
    this.loadInvoices();
  }

  onStartDateChange(date: string) {
    console.log('📅 Data inicial:', date);
    this.currentParams.update(params => ({ 
      ...params, 
      startDate: date || undefined, 
      pageNumber: 1 
    }));
    this.loadInvoices();
  }

  onEndDateChange(date: string) {
    console.log('�� Data final:', date);
    this.currentParams.update(params => ({ 
      ...params, 
      endDate: date || undefined, 
      pageNumber: 1 
    }));
    this.loadInvoices();
  }

  onSortChange(sort: {column: string, direction: string}) {
    console.log('🔄 Ordenando por:', sort);
    // TODO: Implementar sorting no backend
    // Por enquanto, apenas logamos a ação
  }

downloadInvoice(invoice: ClientInvoice) {
  if (this.downloadingInvoice() === invoice.billingInvoiceId) return;
  
  this.downloadingInvoice.set(invoice.billingInvoiceId);
  
  this.clientAreaService.downloadInvoice(invoice.billingInvoiceId).subscribe({
    next: (blob: Blob) => {
      const url = window.URL.createObjectURL(blob);
      const link = document.createElement('a');
      link.href = url;
      link.download = `fatura-${invoice.invoiceNumber}.pdf`;
      link.click();
      window.URL.revokeObjectURL(url);
      this.downloadingInvoice.set(null);
    },
    error: (err) => {
      console.error('Erro ao baixar fatura:', err);
      this.downloadingInvoice.set(null);
    }
  });
}

onActionClick(event: TableAction<unknown>) {
  const typedEvent = event as TableAction<ClientInvoice>;
  if (typedEvent.action === 'download') {
    this.downloadInvoice(typedEvent.item);
  }
}
}