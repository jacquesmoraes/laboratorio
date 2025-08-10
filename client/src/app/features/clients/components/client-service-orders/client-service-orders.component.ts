import { Component, input, signal, computed, inject, OnInit, OnDestroy, DestroyRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { Subject, debounceTime } from 'rxjs';
import { ServiceOrdersService } from '../../../service-order/services/service-order.service';
import { ServiceOrder, ServiceOrderParams, Pagination, OrderStatus, OrderStatusLabels } from '../../../service-order/models/service-order.interface';
import { MatIconModule } from '@angular/material/icon';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormControl, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-client-service-orders',
  standalone: true,
  imports: [CommonModule, MatIconModule, ReactiveFormsModule],
  templateUrl: './client-service-orders.component.html',
  styleUrls: ['./client-service-orders.component.scss']
})
export class ClientServiceOrdersComponent implements OnInit {
  private serviceOrdersService = inject(ServiceOrdersService);
  private router = inject(Router);
    private readonly destroyRef = inject(DestroyRef);
  // Input para receber o clientId do componente pai
  clientId = input.required<number>();
  searchControl = new FormControl<string>('', { nonNullable: true });
  loading = signal(false);
  serviceOrders = signal<ServiceOrder[]>([]);
  pagination = signal<Pagination<ServiceOrder> | null>(null);
  
  
  filters = computed(() => ({
    pageNumber: 1,
    pageSize: 5,
    clientId: this.clientId(),
    search: '',
    sort: '-dateIn'
  } as ServiceOrderParams));

  ngOnInit(): void {
    this.setupSearchDebounce();
    this.loadServiceOrders();
  }

  private setupSearchDebounce(): void {
  this.searchControl.valueChanges
    .pipe(
      debounceTime(500),
      takeUntilDestroyed(this.destroyRef)
    )
    .subscribe(searchTerm => {
      this.loadServiceOrders(searchTerm);
    });
}

  private loadServiceOrders(searchTerm?: string): void {
    this.loading.set(true);

    const currentFilters = this.filters();
    const params: ServiceOrderParams = {
      ...currentFilters,
      search: searchTerm || currentFilters.search
    };

    this.serviceOrdersService.getServiceOrders(params)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (response) => {
          this.serviceOrders.set(response.data);
          this.pagination.set(response);
          this.loading.set(false);
        },
        error: (error) => {
          console.error('Erro ao carregar ordens de serviço:', error);
          this.loading.set(false);
        }
      });
  }

 

  onPageChange(page: number): void {
    const currentFilters = this.filters();
    const params: ServiceOrderParams = {
      ...currentFilters,
      pageNumber: page
    };

    this.loading.set(true);
    this.serviceOrdersService.getServiceOrders(params)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (response) => {
          this.serviceOrders.set(response.data);
          this.pagination.set(response);
          this.loading.set(false);
        },
        error: (error) => {
          console.error('Erro ao carregar ordens de serviço:', error);
          this.loading.set(false);
        }
      });
  }

  clearFilters(): void {
    this.searchControl.setValue('');
  }

  viewServiceOrder(id: number): void {
    this.router.navigate(['/service-orders', id]);
  }

  formatDate(date: string): string {
    return new Date(date).toLocaleDateString('pt-BR');
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL'
    }).format(value);
  }

  protected readonly getStatusClass = computed(() => {
    return (status?: OrderStatus | string | null): string => {
      if (!status) return 'status-unknown';

      // Se for string, tenta converter para OrderStatus
      if (typeof status === 'string') {
        const statusKey = status as keyof typeof OrderStatus;
        if (statusKey in OrderStatus) {
          status = OrderStatus[statusKey];
        }
      }

      // Se for OrderStatus enum
      if (typeof status === 'number') {
        switch (status) {
          case OrderStatus.Production:
            return 'status-production';
          case OrderStatus.TryIn:
            return 'status-tryin';
          case OrderStatus.Finished:
            return 'status-finished';
          default:
            return 'status-unknown';
        }
      }

      return 'status-unknown';
    };
  });

  protected readonly getStatusLabel = computed(() => {
    return (status?: OrderStatus | string | null): string => {
      if (!status) return 'N/A';

      // Se for string, tenta converter para OrderStatus
      if (typeof status === 'string') {
        const statusKey = status as keyof typeof OrderStatus;
        if (statusKey in OrderStatus) {
          status = OrderStatus[statusKey];
        }
      }

      // Se for OrderStatus enum
      if (typeof status === 'number') {
        return OrderStatusLabels[status as OrderStatus] || 'N/A';
      }

      return String(status);
    };
  });

  protected readonly paginationInfo = computed(() => {
    const p = this.pagination();
    if (!p) return null;
    
    return {
      start: (p.pageNumber - 1) * p.pageSize + 1,
      end: Math.min(p.pageNumber * p.pageSize, p.totalItems),
      total: p.totalItems
    };
  });
}