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
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';

import { ServiceOrderDetails, OrderStatus, OrderStatusLabels } from '../../models/service-order.interface';
import { ServiceOrdersService } from '../../services/service-order.service';
import Swal from 'sweetalert2';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ScheduleService } from '../../../../core/services/schedule.service';
import { ScheduleItemRecord } from '../../../../core/models/schedule.model';
import { ScheduleDeliveryModalComponent } from '../schedule-delivery-modal/schedule-delivery-modal.component';
import { SectorService } from '../../../sectors/service/sector.service';

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
private scheduleService = inject(ScheduleService);
private dialog = inject(MatDialog);
private snackBar = inject(MatSnackBar);
private sectorService = inject(SectorService);

  serviceOrder = signal<ServiceOrderDetails | null>(null);
  loading = signal(true);
  schedule = signal<ScheduleItemRecord | null>(null);

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
        this.scheduleService.getActiveScheduleByServiceOrder(id).subscribe({
        next: (sched) => this.schedule.set(sched),
        error: () => this.schedule.set(null)
      });
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

   // ... existing code ...

updateSchedule() {
  const order = this.serviceOrder();
  if (!order) return;

  // Primeiro, carregar os setores
  this.sectorService.getAll().subscribe({
    next: (sectors) => {
      const sectorsData = sectors.map(s => ({ 
        sectorId: s.id, 
        sectorName: s.name 
      }));

      // Depois, buscar o agendamento ativo
      this.scheduleService.getActiveScheduleByServiceOrder(order.serviceOrderId).subscribe({
        next: (schedule: ScheduleItemRecord) => {
          // Modo edição - passar o agendamento completo
          const dialogRef = this.dialog.open(ScheduleDeliveryModalComponent, {
            width: '400px',
            data: {
              serviceOrderId: order.serviceOrderId,
              scheduleId: schedule.scheduleId,
              existingSchedule: schedule, // Passar o agendamento completo
              sectors: sectorsData
            }
          });

          dialogRef.afterClosed().subscribe((success) => {
            if (success) {
              this.snackBar.open('Agendamento atualizado!', 'Fechar', { duration: 3000 });
            }
          });
        },
        error: () => {
          // Modo criação - não tem agendamento existente
          const dialogRef = this.dialog.open(ScheduleDeliveryModalComponent, {
            width: '400px',
            data: {
              serviceOrderId: order.serviceOrderId,
              sectors: sectorsData
            }
          });

          dialogRef.afterClosed().subscribe((success) => {
            if (success) {
              this.snackBar.open('Agendamento criado!', 'Fechar', { duration: 3000 });
            }
          });
        }
      });
    },
    error: () => {
      this.snackBar.open('Erro ao carregar setores', 'Fechar', { duration: 3000 });
    }
  });
}

// ... existing code ...

 getStatusColorClass(status: string | OrderStatus): string {
  const enumValue = typeof status === 'string'
    ? OrderStatus[status as keyof typeof OrderStatus]
    : status;

  switch (enumValue) {
    case OrderStatus.Production: return 'status-production';
    case OrderStatus.TryIn: return 'status-tryin';
    case OrderStatus.Finished: return 'status-finished';
    default: return 'status-default';
  }
}


  getStatusLabel(status: string | OrderStatus | undefined): string {
  if (!status) return '';
  const enumValue = typeof status === 'string'
    ? OrderStatus[status as keyof typeof OrderStatus]
    : status;
  return OrderStatusLabels[enumValue as OrderStatus] ?? 'Desconhecido';
}

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL'
    }).format(value);
  }
}