import { Component, signal, inject, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatDividerModule } from '@angular/material/divider';
import { MatListModule } from '@angular/material/list';
import { MatExpansionModule } from '@angular/material/expansion';
import { SweetAlert2Module } from '@sweetalert2/ngx-sweetalert2';

import { ServiceOrderDetails, OrderStatus, OrderStatusLabels } from '../../models/service-order.interface';
import { ServiceOrdersService } from '../../services/service-order.service';
import Swal from 'sweetalert2';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDialogModule } from '@angular/material/dialog';

@Component({
  selector: 'app-service-order-details',
  standalone: true,
  imports: [
   CommonModule,
  MatCardModule,
  MatButtonModule,
  MatIconModule,
  MatChipsModule,
  MatDividerModule,
  MatListModule,
  MatExpansionModule,
  MatDialogModule,
  MatProgressSpinnerModule,
  SweetAlert2Module
  ],
  templateUrl: './service-order-details.component.html',
  styleUrls: ['./service-order-details.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ServiceOrderDetailsComponent implements OnInit {
  private serviceOrdersService = inject(ServiceOrdersService);
  private route = inject(ActivatedRoute);
  public router = inject(Router);

  serviceOrder = signal<ServiceOrderDetails | null>(null);
  loading = signal(true);

  OrderStatusLabels = OrderStatusLabels;

  ngOnInit() {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (id) {
      this.loadServiceOrder(id);
    }
  }

  loadServiceOrder(id: number) {
    this.loading.set(true);
    this.serviceOrdersService.getServiceOrderById(id).subscribe({
      next: (order) => {
        this.serviceOrder.set(order);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        Swal.fire('Erro', 'Erro ao carregar ordem de serviço', 'error');
      }
    });
  }

  editOrder() {
    const order = this.serviceOrder();
    if (order) {
      this.router.navigate(['service-orders', order.serviceOrderId, 'edit']);
    }
  }

  deleteOrder() {
    const order = this.serviceOrder();
    if (!order) return;

    Swal.fire({
      title: 'Excluir ordem?',
      text: 'Tem certeza que deseja excluir esta ordem?',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: 'Sim, excluir!',
      cancelButtonText: 'Cancelar'
    }).then(result => {
      if (result.isConfirmed) {
        this.serviceOrdersService.deleteServiceOrder(order.serviceOrderId).subscribe({
          next: () => {
            Swal.fire('Excluída!', 'Ordem excluída com sucesso.', 'success');
            this.router.navigate(['service-orders']);
          },
          error: () => {
            Swal.fire('Erro', 'Erro ao excluir ordem de serviço.', 'error');
          }
        });
      }
    });
  }

  getStatusColor(status: OrderStatus): string {
    switch (status) {
      case OrderStatus.Production: return 'var(--accent)';
      case OrderStatus.TryIn: return 'var(--secondary)';
      case OrderStatus.Finished: return 'var(--primary)';
      default: return 'var(--dark)';
    }
  }

  getStatusLabel(status: OrderStatus | undefined): string {
  if (status === undefined) return '';
  return OrderStatusLabels[status];
}

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL'
    }).format(value);
  }
}
