import {
  ChangeDetectionStrategy,
  Component,
  inject,
  signal
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Payment, PaymentParams } from '../../payments/models/payment.interface';
import { Pagination } from '../../service-order/models/service-order.interface';
import { ClientAreaService } from '../services/client-area.services';


@Component({
  selector: 'app-client-area-payments',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <section class="payments">
      <h2>Pagamentos</h2>

      <form class="filters" (ngSubmit)="onSearch()">
        <label>
          Buscar:
          <input type="text" [(ngModel)]="params.search" name="search" placeholder="Descrição..." />
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
            <th>Data</th>
            <th>Descrição</th>
            <th>Valor Pago</th>
          </tr>
        </thead>
        <tbody>
          @for (payment of payments(); track payment.id) {
            <tr>
              <td>{{ payment.paymentDate | date }}</td>
              <td>{{ payment.description }}</td>
              <td>{{ payment.amountPaid | currency:'BRL' }}</td>
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
    .payments {
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
export class ClientAreaPaymentsComponent {
  private readonly service = inject(ClientAreaService);

  readonly payments = signal<Payment[]>([]);
  readonly totalPages = signal<number>(1);

  readonly params: PaymentParams = {
    pageNumber: 1,
    pageSize: 10,
    sort: '-paymentDate',
    search: undefined,
    startDate: undefined,
    endDate: undefined
  };

  constructor() {
    this.loadPayments();
  }

  onSearch() {
    this.params.pageNumber = 1;
    this.loadPayments();
  }

  loadPayments() {
    this.service.getPayments(this.params).subscribe({
      next: (result: Pagination<Payment>) => {
        this.payments.set(result.data);
        this.totalPages.set(result.totalPages);
      }
    });
  }

  nextPage() {
    if (this.params.pageNumber < this.totalPages()) {
      this.params.pageNumber++;
      this.loadPayments();
    }
  }

  prevPage() {
    if (this.params.pageNumber > 1) {
      this.params.pageNumber--;
      this.loadPayments();
    }
  }
}
