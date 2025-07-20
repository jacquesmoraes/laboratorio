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
    <section class="invoices">
      <h2>Faturas</h2>

      <form class="filters" (ngSubmit)="onSearch()">
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

        <button type="submit">Filtrar</button>
      </form>

      <table>
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
              <td>{{ invoice.invoiceNumber }}</td>
              <td>{{ invoice.createdAt | date }}</td>
              <td>{{ invoice.description || 'Sem descrição' }}</td>
              <td>{{ invoice.totalInvoiceAmount | currency:'BRL' }}</td>
              <td>{{ invoiceStatusLabels[invoice.status] }}</td>
              <td>
                <a (click)="downloadPdf(invoice.billingInvoiceId)">Baixar PDF</a>
              </td>
            </tr>
          }
        </tbody>
      </table>

      <footer class="pagination">
        <button (click)="prevPage()" [disabled]="params.pageNumber === 1">&laquo; Anterior</button>
        <span>Página {{ params.pageNumber }} de {{ totalPages() }}</span>
        <button (click)="nextPage()" [disabled]="params.pageNumber === totalPages()">&raquo; Próxima</button>
      </footer>
    </section>
  `,
  styles: [`
    .invoices {
      padding: 1rem;
      background-color: #f4f1ee;
      color: #334a52;
      border-radius: .5rem;
    }

    h2 {
      color: #276678;
    }

    .filters {
      display: flex;
      gap: 1rem;
      margin-bottom: 1rem;
    }

    table {
      width: 100%;
      border-collapse: collapse;
    }

    th, td {
      border: 1px solid #96afb8;
      padding: .5rem;
      text-align: left;
    }

    .pagination {
      margin-top: 1rem;
      display: flex;
      justify-content: center;
      gap: 1rem;
    }
  `],
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
