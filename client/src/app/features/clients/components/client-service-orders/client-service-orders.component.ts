import { Component, input, signal, computed, inject, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { Subject, debounceTime, takeUntil } from 'rxjs';
import { ServiceOrdersService } from '../../../service-order/services/service-order.service';
import { ServiceOrder, ServiceOrderParams, Pagination, OrderStatus, OrderStatusLabels } from '../../../service-order/models/service-order.interface';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-client-service-orders',
  standalone: true,
  imports: [CommonModule, MatIconModule],
  templateUrl: './client-service-orders.component.html',
  styleUrls: ['./client-service-orders.component.scss']
})
export class ClientServiceOrdersComponent implements OnInit, OnDestroy {
  private serviceOrdersService = inject(ServiceOrdersService);
  private router = inject(Router);
  private destroy$ = new Subject<void>();
  private searchSubject = new Subject<string>();

  // Input para receber o clientId do componente pai
  clientId = input.required<number>();

  loading = signal(false);
  serviceOrders = signal<ServiceOrder[]>([]);
  pagination = signal<Pagination<ServiceOrder> | null>(null);
  Math = Math;
  // ✅ CORREÇÃO: Usar computed para garantir que clientId esteja disponível
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

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private setupSearchDebounce(): void {
    this.searchSubject.pipe(
      debounceTime(500),
      takeUntil(this.destroy$)
    ).subscribe(searchTerm => {
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

    this.serviceOrdersService.getServiceOrders(params).subscribe({
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

  onSearchChange(value: string): void {
    this.searchSubject.next(value);
  }

  onPageChange(page: number): void {
    const currentFilters = this.filters();
    const params: ServiceOrderParams = {
      ...currentFilters,
      pageNumber: page
    };
    
    this.loading.set(true);
    this.serviceOrdersService.getServiceOrders(params).subscribe({
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
    this.loadServiceOrders('');
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

  getStatusClass(status?: OrderStatus | string | null): string {
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
  }
  
  getStatusLabel(status?: OrderStatus | string | null): string {
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
  }
}