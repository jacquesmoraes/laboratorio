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
      <table mat-table [dataSource]="serviceOrders()" class="service-order-table">
        <!-- Seleção -->
        <ng-container matColumnDef="select">
          <th mat-header-cell *matHeaderCellDef>
            <mat-icon>check_box_outline_blank</mat-icon>
          </th>
          <td mat-cell *matCellDef="let order">
            <mat-checkbox 
              [checked]="selectedOrderIds().includes(order.serviceOrderId)"
              (change)="onSelectionChange(order.serviceOrderId)" 
              color="primary">
            </mat-checkbox>
          </td>
        </ng-container>

        <!-- Número da Ordem -->
        <ng-container matColumnDef="orderNumber">
          <th mat-header-cell *matHeaderCellDef>Número</th>
          <td mat-cell *matCellDef="let order">{{ order.orderNumber }}</td>
        </ng-container>

        <!-- Cliente -->
        <ng-container matColumnDef="clientName">
          <th mat-header-cell *matHeaderCellDef>Cliente</th>
          <td mat-cell *matCellDef="let order">
            <a 
              [routerLink]="['/clients', order.clientId]" 
              class="client-link" 
              (click)="$event.stopPropagation()">
              {{ order.clientName }}
            </a>
          </td>
        </ng-container>

        <!-- Paciente -->
        <ng-container matColumnDef="patientName">
          <th mat-header-cell *matHeaderCellDef>Paciente</th>
          <td mat-cell *matCellDef="let order">{{ order.patientName }}</td>
        </ng-container>

        <!-- Data de Entrada -->
        <ng-container matColumnDef="dateIn">
          <th mat-header-cell *matHeaderCellDef>Data Entrada</th>
          <td mat-cell *matCellDef="let order">{{ order.dateIn | date: 'dd/MM/yyyy' }}</td>
        </ng-container>

        <!-- Última Movimentação -->
        <ng-container matColumnDef="lastMovementDate">
          <th mat-header-cell *matHeaderCellDef>Última Movimentação</th>
          <td mat-cell *matCellDef="let order">
            <span *ngIf="order.lastMovementDate; else noMovement">
              {{ order.lastMovementDate | date: 'dd/MM/yyyy' }}
            </span>
            <ng-template #noMovement>
              <span class="text-muted">-</span>
            </ng-template>
          </td>
        </ng-container>

        <!-- Status -->
        <ng-container matColumnDef="status">
          <th mat-header-cell *matHeaderCellDef>Status</th>
          <td mat-cell *matCellDef="let order">
            <mat-chip [class]="getStatusColorClass(order.status)" class="status-chip">
              {{ getStatusLabel(order.status) }}
            </mat-chip>
          </td>
        </ng-container>

        <!-- Setor Atual -->
        <ng-container matColumnDef="currentSectorName">
          <th mat-header-cell *matHeaderCellDef>Setor Atual</th>
          <td mat-cell *matCellDef="let order">{{ order.currentSectorName || '-' }}</td>
        </ng-container>

        <!-- Ações -->
        <ng-container matColumnDef="actions">
          <th mat-header-cell *matHeaderCellDef>Ações</th>
          <td mat-cell *matCellDef="let order">
            <button 
              mat-icon-button 
              class="action-btn" 
              (click)="onAction('view', order.serviceOrderId)"
              matTooltip="Ver detalhes">
              <mat-icon>visibility</mat-icon>
            </button>
            
            <button 
              mat-icon-button 
              class="action-btn" 
              (click)="onAction('edit', order.serviceOrderId)" 
              matTooltip="Editar">
              <mat-icon>edit</mat-icon>
            </button>
            
            <button 
              mat-icon-button 
              class="action-btn" 
              (click)="onAction('sendToTryIn', order.serviceOrderId)"
              matTooltip="Enviar para Prova" 
              [disabled]="order.status === 'TryIn' || order.status === 'Finished'">
              <mat-icon>send</mat-icon>
            </button>
            
            <button 
              mat-icon-button 
              class="action-btn" 
              (click)="onAction('moveToStage', order.serviceOrderId)"
              matTooltip="Mudar de Setor" 
              [disabled]="order.status === 'Finished'">
              <mat-icon>swap_horiz</mat-icon>
            </button>
            
            <button 
              mat-icon-button 
              class="action-btn" 
              (click)="onAction('reopen', order.serviceOrderId)"
              matTooltip="Reabrir OS" 
              [disabled]="order.status !== 'Finished'">
              <mat-icon>refresh</mat-icon>
            </button>
          </td>
        </ng-container>

        <!-- Linhas da tabela -->
        <tr mat-header-row *matHeaderRowDef="displayedColumns" class="table-header-row"></tr>
        <tr mat-row *matRowDef="let row; columns: displayedColumns;" class="table-row"></tr>
      </table>

      <!-- Paginador -->
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
  // Inputs
  readonly serviceOrders = input.required<ServiceOrder[]>();
  readonly selectedOrderIds = input.required<number[]>();
  readonly totalItems = input.required<number>();
  readonly pageSize = input.required<number>();
  readonly pageIndex = input.required<number>();
  readonly pageSizeOptions = input.required<number[]>();
  readonly loading = input<boolean>(false);

  // Outputs
  readonly selectionChange = output<number>();
  readonly pageChange = output<PageEvent>();
  readonly action = output<TableActionEvent>();

  // Configurações
  readonly displayedColumns = SERVICE_ORDER_LIST_CONFIG.displayedColumns;
  readonly statusMap = SERVICE_ORDER_LIST_CONFIG.statusMap;
  readonly statusClasses = SERVICE_ORDER_LIST_CONFIG.statusClasses;

  // Métodos
  onSelectionChange(orderId: number) {
    this.selectionChange.emit(orderId);
  }

  onPageChange(event: PageEvent) {
    this.pageChange.emit(event);
  }

  onAction(type: TableActionEvent['type'], orderId: number) {
    this.action.emit({ type, orderId });
  }

  getStatusColorClass(status: string): string {
    const statusNum = this.statusMap[status] ?? -1;
    return this.statusClasses[statusNum] ?? 'status-default';
  }

  getStatusLabel(status: string): string {
    const statusNum = this.statusMap[status];
    return statusNum ? this.getStatusLabelFromEnum(statusNum) : status;
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