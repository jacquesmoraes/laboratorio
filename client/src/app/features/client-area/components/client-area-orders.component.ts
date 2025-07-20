import {
  ChangeDetectionStrategy,
  Component,
  inject,
  signal
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {  Pagination } from '../../service-order/models/service-order.interface';
import { ClientAreaService } from '../services/client-area.services';
import { 
  ClientAreaServiceOrder, 
  ClientAreaOrderStatus,
  orderStatusLabels, 
  orderStatusValues, 
  ServiceOrderParams
} from '../../../core/models/client-area.model';

@Component({
  selector: 'app-client-area-orders',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
     <section class="orders">
      <h2>Ordens de Serviço</h2>

      <form class="filters" (ngSubmit)="onSearch()">
        <label>
          Buscar:
          <input type="text" [(ngModel)]="params.search" name="search" placeholder="Paciente..." />
        </label>

        <label>
          Status:
          <select [(ngModel)]="params.status" name="status">
            <option [ngValue]="undefined">Todos</option>
            @for (s of orderStatusValues; track s) {
              <option [ngValue]="s">
                {{ orderStatusLabels[s] }}
              </option>
            }
          </select>
        </label>

        <button type="submit">Filtrar</button>
      </form>

      <table>
        <thead>
          <tr>
            <th>Data Entrada</th>
            <th>Paciente</th>
            <th>Status</th>
            <th>Total</th>
          </tr>
        </thead>
        <tbody>
          @for (order of orders(); track order.serviceOrderId) {
            <tr>
              <td>{{ order.dateIn | date }}</td>
              <td>{{ order.patientName }}</td>
              <td>{{ orderStatusLabels[order.status] }}</td>
              <td>{{ order.orderTotal | currency:'BRL' }}</td>
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
    .orders {
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
export class ClientAreaOrdersComponent {
  private readonly service = inject(ClientAreaService);

  readonly orders = signal<ClientAreaServiceOrder[]>([]);
  readonly totalPages = signal<number>(1);
  readonly orderStatusLabels = orderStatusLabels;
  readonly orderStatusValues = orderStatusValues;

  readonly params: ServiceOrderParams = {
    pageNumber: 1,
    pageSize: 10,
    sort: '-dateIn',
    search: undefined,
    status: undefined,
    clientId: undefined
  };

  constructor() {
    this.loadOrders();
  }

  onSearch() {
    this.params.pageNumber = 1;
    this.loadOrders();
  }

  loadOrders() {
  console.log('Loading orders with params:', this.params);
  this.service.getOrders(this.params).subscribe({
    next: (result: Pagination<ClientAreaServiceOrder>) => {
      console.log('Orders response:', result);
      this.orders.set(result.data);
      this.totalPages.set(result.totalPages);
      console.log('Total pages set to:', result.totalPages);
    },
    error: (error) => {
      console.error('Error loading orders:', error);
    }
  });
}

nextPage() {
  console.log('Next page clicked. Current:', this.params.pageNumber, 'Total:', this.totalPages());
  if (this.params.pageNumber < this.totalPages()) {
    this.params.pageNumber++;
    console.log('Moving to page:', this.params.pageNumber);
    this.loadOrders();
  }
}

prevPage() {
  console.log('Prev page clicked. Current:', this.params.pageNumber);
  if (this.params.pageNumber > 1) {
    this.params.pageNumber--;
    console.log('Moving to page:', this.params.pageNumber);
    this.loadOrders();
  }
}
}