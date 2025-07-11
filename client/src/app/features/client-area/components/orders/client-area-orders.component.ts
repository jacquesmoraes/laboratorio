// client/src/app/features/client-area/components/orders/client-area-orders.component.ts
import { Component, OnInit, signal, inject, ChangeDetectionStrategy, input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

import { ClientAreaService } from '../../services/client-area.service';
import { OrderParams, ServiceOrder } from '../../models/client-area.interface';

@Component({
  selector: 'app-client-area-orders',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './client-area-orders.component.html',
  styleUrls: ['./client-area-orders.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ClientAreaOrdersComponent implements OnInit {
  private readonly clientAreaService = inject(ClientAreaService);

  // Inputs para reutilização
  limit = input<number>();
  showPagination = input<boolean>(true);
  showHeader = input<boolean>(true);
  showViewAllLink = input<boolean>(true);

  orders = signal<ServiceOrder[]>([]);
  isLoading = signal(true);
  currentPage = signal(1);
  totalPages = signal(1);
  totalOrders = signal(0);
  itemsPerPage = 10;

  ngOnInit() {
    this.loadOrders();
  }

  loadOrders(page: number = 1) {
  this.isLoading.set(true);

  const params: OrderParams = {
    pageNumber: page,
    pageSize: this.limit() || this.itemsPerPage
  };

  this.clientAreaService.getOrders(params).subscribe({
    next: (res) => {
      console.log('Dados das ordens recebidos:', res.data);
      console.log('Primeira ordem:', res.data[0]);
      
      this.orders.set(res.data);
      this.currentPage.set(res.pageNumber);
      this.totalPages.set(res.totalPages);
      this.totalOrders.set(res.totalItems);
      this.isLoading.set(false);
    },
    error: (err) => {
      console.error('Erro ao carregar ordens de serviço', err);
      this.isLoading.set(false);
    }
  });
}

  onPageChange(page: number) {
    this.loadOrders(page);
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(value);
  }

  formatDate(date: string): string {
    return new Date(date).toLocaleDateString('pt-BR');
  }

  getStatusClass(status: string): string {
    switch (status.toLowerCase()) {
      case 'completed':
      case 'concluído':
        return 'completed';
      case 'pending':
      case 'pendente':
        return 'pending';
      case 'in_progress':
      case 'em_andamento':
        return 'in-progress';
      case 'cancelled':
      case 'cancelado':
        return 'cancelled';
      default:
        return 'pending';
    }
  }
}