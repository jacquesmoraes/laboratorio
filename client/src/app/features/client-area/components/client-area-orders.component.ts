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
} from '../models/client-area.model';
import { Router } from '@angular/router';

@Component({
  selector: 'app-client-area-orders',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
     <section class="client-area-section">
      <h2>Ordens de Serviço</h2>

      <form class="client-area-filters" (ngSubmit)="onSearch()">
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
        <div class="filter-buttons">
          <button type="submit">Filtrar</button>
          <button type="button" (click)="clearFilters()" class="clear-filters-btn">Limpar Filtros</button>
        </div>
      </form>

      <table class="client-area-table">
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
    <tr (click)="goToDetails(order.serviceOrderId)" class="clickable-row">
      <td data-label="Entrada">{{ order.dateIn | date:'dd/MM/yyyy' }}</td>
      <td data-label="Paciente">{{ order.patientName }}</td>
      <td data-label="Status">
        <span class="client-area-status" [class]="getOrderStatusClass(order.status)">
          {{ orderStatusLabels[order.status] }}
        </span>
      </td>
      <td data-label="Total">{{ order.orderTotal | currency:'BRL' }}</td>
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
export class ClientAreaOrdersComponent {
  private readonly service = inject(ClientAreaService);
private readonly router = inject(Router);
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

  clearFilters() {
    this.params.search = undefined;
    this.params.status = undefined;
    this.params.pageNumber = 1;
    this.loadOrders();
  }

  loadOrders() {
  
  this.service.getOrders(this.params).subscribe({
    next: (result: Pagination<ClientAreaServiceOrder>) => {
      
      this.orders.set(result.data);
      this.totalPages.set(result.totalPages);
      
    },
    error: (error) => {
      console.error('Error loading orders:', error);
    }
  });
}

nextPage() {
  
  if (this.params.pageNumber < this.totalPages()) {
    this.params.pageNumber++;
    
    this.loadOrders();
  }
}

prevPage() {
  console.log('Prev page clicked. Current:', this.params.pageNumber);
  if (this.params.pageNumber > 1) {
    this.params.pageNumber--;
    
    this.loadOrders();
  }
}
goToDetails(orderId: number) {
  this.router.navigate([`/client-area/orders/${orderId}`]);
}

  getOrderStatusClass(status: ClientAreaOrderStatus): string {
    switch (status) {
      case 'Production':
        return 'client-area-status-production';
      case 'TryIn':
        return 'client-area-status-tryin';
      case 'Finished':
        return 'client-area-status-finished';
      default:
        return 'client-area-status-production';
    }
  }
}