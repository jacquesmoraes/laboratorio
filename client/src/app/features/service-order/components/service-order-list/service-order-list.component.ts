import {
  Component,
  signal,
  computed,
  inject,
  OnInit,
  OnDestroy,
  ChangeDetectionStrategy,
} from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormControl, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatCheckboxModule } from '@angular/material/checkbox';

import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';
import { Subject } from 'rxjs';

import {
  ServiceOrder,
  ServiceOrderParams,
  OrderStatus,
  OrderStatusLabels,
} from '../../models/service-order.interface';
import { ServiceOrdersService } from '../../services/service-order.service';

@Component({
  selector: 'app-service-order-list',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    MatTableModule,
    MatPaginatorModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatCheckboxModule,
    MatCardModule,
    MatChipsModule,
    RouterModule,
    MatTooltipModule,
  ],
  templateUrl: './service-order-list.component.html',
  styleUrls: ['./service-order-list.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ServiceOrderListComponent implements OnInit, OnDestroy {
  private readonly serviceOrdersService = inject(ServiceOrdersService);
  private readonly router = inject(Router);
  private readonly destroy$ = new Subject<void>();

  serviceOrders = signal<ServiceOrder[]>([]);
  selectedOrderIds = signal<number[]>([]);
  pagination = signal<any>(null);
  loading = signal(false);

  searchControl = new FormControl('');
  selectedStatus: OrderStatus | '' = '';
  sortBy = 'DateIn';

  pageSize = signal(10);
  currentPage = signal(1);

  private readonly initialParams: ServiceOrderParams = {
    pageNumber: 1,
    pageSize: 10,
    sort: 'DateIn',
    search: '',
  };

  currentParams = signal<ServiceOrderParams>({ ...this.initialParams });

  readonly displayedColumns = [
    'select',
    'orderNumber',
    'clientName',
    'patientName',
    'dateIn',
    'status',
    'currentSectorName',
    'actions',
  ];

  readonly statusMap: Record<string, OrderStatus> = {
  Production: OrderStatus.Production,
  TryIn: OrderStatus.TryIn,
  Finished: OrderStatus.Finished,
};

  readonly orderStatuses = [
    { value: OrderStatus.Production, label: OrderStatusLabels[OrderStatus.Production] },
    { value: OrderStatus.TryIn, label: OrderStatusLabels[OrderStatus.TryIn] },
    { value: OrderStatus.Finished, label: OrderStatusLabels[OrderStatus.Finished] },
  ];

  readonly statusClasses: Record<number, string> = {
    [OrderStatus.Production]: 'status-production',
    [OrderStatus.TryIn]: 'status-tryin',
    [OrderStatus.Finished]: 'status-finished',
  };

  totalPages = computed(() => this.pagination()?.totalPages || 1);
  totalItems = computed(() => this.pagination()?.totalItems || 0);
  hasSelection = computed(() => this.selectedOrderIds().length > 0);

  ngOnInit() {
    this.loadServiceOrders();
    this.setupSearch();
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private setupSearch() {
    this.searchControl.valueChanges
      .pipe(debounceTime(300), distinctUntilChanged(), takeUntil(this.destroy$))
      .subscribe((search) => {
        this.currentParams.update((params) => ({
          ...params,
          search: search || '',
          pageNumber: 1,
        }));
        this.loadServiceOrders();
      });
  }

  loadServiceOrders() {
    this.loading.set(true);
    this.serviceOrdersService.getServiceOrders(this.currentParams()).subscribe({
      next: (res) => {
        this.pagination.set(res);
        this.serviceOrders.set(res.data);
        this.selectedOrderIds.set([]);
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Error loading service orders:', err);
        this.loading.set(false);
      },
    });
  }

  toggleSelection(id: number) {
    const current = this.selectedOrderIds();
    if (current.includes(id)) {
      this.selectedOrderIds.set(current.filter((x) => x !== id));
    } else {
      this.selectedOrderIds.set([...current, id]);
    }
  }

  onStatusChange() {
    this.currentParams.update((params) => ({
      ...params,
      status: this.selectedStatus ? Number(this.selectedStatus) : undefined,
      pageNumber: 1,
    }));
    this.loadServiceOrders();
  }

  onSortChange() {
    this.currentParams.update((params) => ({
      ...params,
      sort: this.sortBy,
    }));
    this.loadServiceOrders();
  }

  onPageChange(event: PageEvent) {
    this.currentParams.update((params) => ({
      ...params,
      pageNumber: event.pageIndex + 1,
      pageSize: event.pageSize,
    }));
    this.loadServiceOrders();
  }

  navigateToNew() {
    this.router.navigate(['service-orders/new']);
  }

  viewDetails(id: number) {
    this.router.navigate(['service-orders', id]);
  }

  editOrder(id: number) {
    this.router.navigate(['service-orders', id, 'edit']);
  }

 getStatusColorClass(status: string): string {
  const statusNum = this.statusMap[status] ?? -1;
  return this.statusClasses[statusNum] ?? 'status-default';
}



  getStatusLabel(status: string): string {
  const statusNum = this.statusMap[status];
  return statusNum ? OrderStatusLabels[statusNum] : status;
}

}
