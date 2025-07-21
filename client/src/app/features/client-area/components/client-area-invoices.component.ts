import {
  ChangeDetectionStrategy,
  Component,
  inject,
  signal
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { ClientAreaService } from '../services/client-area.services';

import { Pagination } from '../../service-order/models/service-order.interface';
import { ClientAreaInvoice, InvoiceStatus, invoiceStatusLabels, invoiceStatusValues } from '../../../core/models/client-area.model';


@Component({
  selector: 'app-client-area-invoices',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <section class="client-area-section">
      <h2>Faturas</h2>

      <form class="client-area-filters" (ngSubmit)="onSearch()">
        <label>
          Status:
          <select [(ngModel)]="params.status" name="status">
            <option [ngValue]="undefined">Todos</option>
            @for (s of invoiceStatusValues; track s) {
              <option [ngValue]="s">
                {{ invoiceStatusLabels[s] }}
              </option>
            }
          </select>
        </label>

        <label>
          Data início:
          <input type="date" [(ngModel)]="params.startDate" name="startDate" />
        </label>

        <label>
          Data fim:
          <input type="date" [(ngModel)]="params.endDate" name="endDate" />
        </label>

        <div class="filter-buttons">
          <button type="submit">Filtrar</button>
          <button type="button" (click)="clearFilters()" class="clear-filters-btn">Limpar Filtros</button>
        </div>
      </form>

      <table class="client-area-table">
        <thead>
          <tr>
            <th>Número</th>
            <th>Criada em</th>
            <th>Descrição</th>
            <th>Total</th>
            <th>Status</th>
            <th>Ações</th>
          </tr>
        </thead>
        <tbody>
          @for (invoice of invoices(); track invoice.billingInvoiceId) {
            <tr>
              <td data-label="Nº fatura">{{ invoice.invoiceNumber }}</td>
              <td data-label="Criada em:">{{ invoice.createdAt | date:'dd/MM/yyyy' }}</td>
              <td data-label="descrição">{{ invoice.description || 'Sem descrição' }}</td>
              <td data-label="Tota">{{ invoice.totalInvoiceAmount | currency:'BRL' }}</td>
              <td data-label="Status" >
                <span  class="client-area-status" [class]="getStatusClass(invoice.status)">
                  {{ invoiceStatusLabels[invoice.status] }}
                </span>
              </td>
              <td>
                <a class="action-link" (click)="downloadPdf(invoice.billingInvoiceId)">Baixar PDF</a>
              </td>
            </tr>
          }
        </tbody>
      </table>

      <footer class="client-area-pagination">
        <button (click)="prevPage()" [disabled]="params.pageNumber === 1">&laquo; Anterior</button>
        <span>Página {{ params.pageNumber }} de {{ totalPages() }}</span>
        <button (click)="nextPage()" [disabled]="params.pageNumber === totalPages()">&raquo; Próxima</button>
      </footer>
    </section>
  `,
  styleUrls: ['../client-area.styles.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ClientAreaInvoicesComponent {
  private readonly service = inject(ClientAreaService);

  readonly invoices = signal<ClientAreaInvoice[]>([]);
  readonly totalPages = signal<number>(1);
 readonly invoiceStatusLabels = invoiceStatusLabels;
  readonly invoiceStatusValues = invoiceStatusValues;
  readonly params = {
    pageNumber: 1,
    pageSize: 10,
    sort: '-createdAt',
    status: undefined as InvoiceStatus | undefined,
    startDate: undefined as string | undefined,
    endDate: undefined as string | undefined
  };

  constructor() {
    console.log('ClientAreaInvoicesComponent initialized');
    this.loadInvoices();
  }

  onSearch() {
    this.params.pageNumber = 1;
    this.loadInvoices();
  }
  clearFilters() {
    this.params.status = undefined;
    this.params.startDate = undefined;
    this.params.endDate = undefined;
    this.params.pageNumber = 1;
    this.loadInvoices();
  }
  loadInvoices() {
    console.log('Loading invoices with params:', this.params);
    this.service.getInvoices(this.params).subscribe({
      next: (result: Pagination<ClientAreaInvoice>) => {
        console.log('Invoices loaded:', result);
        this.invoices.set(result.data);
        this.totalPages.set(result.totalPages);
      },
      error: (error) => {
        console.error('Erro ao carregar faturas:', error);
      }
    });
  }

  nextPage() {
    if (this.params.pageNumber < this.totalPages()) {
      this.params.pageNumber++;
      this.loadInvoices();
    }
  }

  prevPage() {
    if (this.params.pageNumber > 1) {
      this.params.pageNumber--;
      this.loadInvoices();
    }
  }

  getStatusClass(status: InvoiceStatus): string {
    switch (status) {
      case 'Open':
        return 'client-area-status-open';
      case 'PartiallyPaid':
        return 'client-area-status-partially-paid';
      case 'Paid':
        return 'client-area-status-paid';
      case 'Cancelled':
        return 'client-area-status-cancelled';
      case 'Closed':
        return 'client-area-status-closed';
      default:
        return 'client-area-status-open';
    }
  }

  downloadPdf(id: number) {
    this.service.downloadInvoice(id).subscribe(blob => {
      const url = window.URL.createObjectURL(blob);
      const link = document.createElement('a');
      link.href = url;
      link.download = `invoice-${id}.pdf`;
      link.click();
      window.URL.revokeObjectURL(url);
    });
  }
}
