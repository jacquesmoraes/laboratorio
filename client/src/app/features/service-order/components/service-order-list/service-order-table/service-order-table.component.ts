import { Component, input, output, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { OrderStatus, ServiceOrder } from '../../../models/service-order.interface';
import { SERVICE_ORDER_LIST_CONFIG } from '../models/service-order-list.config';

export interface TableActionEvent {
  type: 'view' | 'edit' | 'sendToTryIn' | 'moveToStage' | 'reopen';
  orderId: number;
}

@Component({
  selector: 'app-service-order-table',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatTableModule,
    MatPaginatorModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatTooltipModule,
    MatCheckboxModule,
  ],
  template: `
    <div class="table-container">
      <table class="service-order-table">
        <thead>
          <tr>
            <th></th>
            <th>Número</th>
            <th>Cliente</th>
            <th>Paciente</th>
            <th>Data Entrada</th>
            <th>Última Movimentação</th>
            <th>Status</th>
            <th>Setor Atual</th>
            <th>Ações</th>
          </tr>
        </thead>
        <tbody>
          <tr *ngFor="let order of serviceOrders()">
            <td data-label="Seleção">
              <mat-checkbox 
                [checked]="selectedOrderIds().includes(order.serviceOrderId)"
                (change)="onSelectionChange(order.serviceOrderId)"
                color="primary">
              </mat-checkbox>
            </td>
            <td data-label="Número">{{ order.orderNumber }}</td>
            <td data-label="Cliente">
              <a [routerLink]="['/clients', order.clientId]" class="client-link" (click)="$event.stopPropagation()">
                {{ order.clientName }}
              </a>
            </td>
            <td data-label="Paciente">{{ order.patientName }}</td>
            <td data-label="Data Entrada">{{ order.dateIn | date: 'dd/MM/yyyy' }}</td>
            <td data-label="Última Movimentação">
            <span *ngIf="order.lastMovement; else noMovement">
    {{ order.lastMovement | date: 'dd/MM/yyyy' }}
  </span>
              <ng-template #noMovement>
                <span class="text-muted">-</span>
              </ng-template>
            </td>
            <td data-label="Status">
             
            <mat-chip [class]="getStatusColorClass(order.status)" class="status-chip">
  {{ getStatusLabel(order.status) }}
</mat-chip>
            </td>
            <td data-label="Setor Atual">{{ order.currentSectorName || '-' }}</td>
            <td data-label="Ações">
              <div class="action-buttons">
                <button mat-icon-button class="action-btn" (click)="onAction('view', order.serviceOrderId)" matTooltip="Ver detalhes">
                  <mat-icon>visibility</mat-icon>
                </button>
                <button mat-icon-button class="action-btn" (click)="onAction('edit', order.serviceOrderId)" matTooltip="Editar">
                  <mat-icon>edit</mat-icon>
                </button>
                <button mat-icon-button class="action-btn" (click)="onAction('sendToTryIn', order.serviceOrderId)" matTooltip="Enviar para Prova" [disabled]="order.status === OrderStatus.TryIn || order.status === OrderStatus.Finished">
  <mat-icon>send</mat-icon>
</button>
<button mat-icon-button class="action-btn" (click)="onAction('moveToStage', order.serviceOrderId)" matTooltip="Mudar de Setor" [disabled]="order.status === OrderStatus.Finished">
  <mat-icon>swap_horiz</mat-icon>
</button>
<button mat-icon-button class="action-btn"
        (click)="onAction('reopen', order.serviceOrderId)"
        matTooltip="Reabrir OS"
        [disabled]="!isFinished(order.status)">
  <mat-icon>refresh</mat-icon>
</button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>

      <mat-paginator 
        [length]="totalItems()" 
        [pageSize]="pageSize()"
        [pageIndex]="pageIndex()" 
        [pageSizeOptions]="pageSizeOptions()" 
        (page)="onPageChange($event)"
        showFirstLastButtons>
      </mat-paginator>
    </div>
  `,
  styleUrls: ['./service-order-table.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ServiceOrderTableComponent {
  readonly serviceOrders = input.required<ServiceOrder[]>();
  readonly selectedOrderIds = input.required<number[]>();
  readonly totalItems = input.required<number>();
  readonly pageSize = input.required<number>();
  readonly pageIndex = input.required<number>();
  readonly pageSizeOptions = input.required<number[]>();
  readonly loading = input<boolean>(false);
  readonly OrderStatus = OrderStatus;
  readonly selectionChange = output<number>();
  readonly pageChange = output<PageEvent>();
  readonly action = output<TableActionEvent>();

  readonly displayedColumns = SERVICE_ORDER_LIST_CONFIG.displayedColumns;
  readonly statusMap = SERVICE_ORDER_LIST_CONFIG.statusMap;
  readonly statusClasses = SERVICE_ORDER_LIST_CONFIG.statusClasses;

  isFinished(status: OrderStatus | string | number): boolean {
    // Resolve status mesmo que venha como string
    const resolved = typeof status === 'string' ? this.statusMap[status] : Number(status);
    return resolved === OrderStatus.Finished;
  }
  onSelectionChange(orderId: number) {
    this.selectionChange.emit(orderId);
  }

  onPageChange(event: PageEvent) {
    this.pageChange.emit(event);
  }

  onAction(type: TableActionEvent['type'], orderId: number) {
    this.action.emit({ type, orderId });
  }

  getStatusLabel(status: OrderStatus): string {
    const statusNum = this.statusMap[status as any];
    return statusNum ? this.getStatusLabelFromEnum(statusNum as OrderStatus) : String(status);
  }

  getStatusColorClass(status: OrderStatus): string {
    const statusNum = this.statusMap[status as any] ?? -1;
    return this.statusClasses[statusNum] ?? 'status-default';
  }


  private getStatusLabelFromEnum(status: OrderStatus): string {
    switch (status) {
      case OrderStatus.Production:
        return 'Em Produção';
      case OrderStatus.TryIn:
        return 'Em Prova';
      case OrderStatus.Finished:
        return 'Finalizado';
      default:
        return 'Desconhecido';
    }
  }
}
