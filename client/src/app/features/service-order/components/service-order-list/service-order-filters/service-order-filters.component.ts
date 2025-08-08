import { Component, input, output, ChangeDetectionStrategy, OnInit, OnDestroy, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, FormGroup, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatIconModule } from '@angular/material/icon';

import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';
import { Subject } from 'rxjs';

import { OrderStatus } from '../../../models/service-order.interface';
import { SERVICE_ORDER_LIST_CONFIG } from '../models/service-order-list.config';

export interface FilterChangeEvent {
  search: string;
  status: OrderStatus | '';
  sortBy: string;
  showFinishedOrders: boolean;
}

@Component({
  selector: 'app-service-order-filters',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatCheckboxModule,
    MatIconModule,
  ],
  template: `
   <div class="filters-section">
  <!-- Busca -->
  <mat-form-field appearance="outline" class="filter-field">
    <mat-label>Buscar</mat-label>
    <input 
      matInput 
      [formControl]="searchControl" 
      placeholder="Buscar por cliente ou paciente..." />
    <mat-icon matSuffix>search</mat-icon>
  </mat-form-field>

  <!-- Filtro por Status -->
  <mat-form-field appearance="outline" class="filter-field">
    <mat-label>Status</mat-label>
    <mat-select 
      [formControl]="statusControl" 
      (selectionChange)="onStatusChange()">
      <mat-option value="">Todos</mat-option>
      @for (status of orderStatuses; track status.value) {
        <mat-option [value]="status.value">
          {{ status.label }}
        </mat-option>
      }
    </mat-select>
  </mat-form-field>

  <!-- Ordenação -->
  <mat-form-field appearance="outline" class="filter-field">
    <mat-label>Ordenar por</mat-label>
    <mat-select 
      [formControl]="sortByControl" 
      (selectionChange)="onSortChange()">
      @for (option of sortOptions; track option.value) {
        <mat-option [value]="option.value">
          {{ option.label }}
        </mat-option>
      }
    </mat-select>
  </mat-form-field>

  <!-- Checkbox para mostrar OS finalizadas -->
  <div class="checkbox-container">
    <mat-checkbox 
      [formControl]="showFinishedOrdersControl" 
      (change)="onShowFinishedOrdersChange()" 
      color="primary">
      Mostrar OS Finalizadas
    </mat-checkbox>
  </div>
</div>
  `,
  styleUrls: ['./service-order-filters.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ServiceOrderFiltersComponent implements OnInit, OnDestroy {
  // Inputs
  readonly initialSearch = input<string>('');
  readonly initialStatus = input<OrderStatus | ''>('');
  readonly initialSortBy = input<string>('DateIn');
  readonly initialShowFinishedOrders = input<boolean>(false);

  // Outputs
  readonly filterChange = output<FilterChangeEvent>();

  // Form controls
  readonly searchControl = new FormControl('');
  readonly statusControl = new FormControl<OrderStatus | ''>('');
  readonly sortByControl = new FormControl('DateIn');
  readonly showFinishedOrdersControl = new FormControl(false);

  // Configurações
  readonly orderStatuses = SERVICE_ORDER_LIST_CONFIG.orderStatuses;
  readonly sortOptions = SERVICE_ORDER_LIST_CONFIG.sortOptions;

  private readonly destroy$ = new Subject<void>();

  ngOnInit() {
    this.initializeForm();
    this.setupSearchDebounce();
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private initializeForm() {
    // Definir valores iniciais
    this.searchControl.setValue(this.initialSearch());
    this.statusControl.setValue(this.initialStatus());
    this.sortByControl.setValue(this.initialSortBy());
    this.showFinishedOrdersControl.setValue(this.initialShowFinishedOrders());
  }

  private setupSearchDebounce() {
    this.searchControl.valueChanges
      .pipe(
        debounceTime(SERVICE_ORDER_LIST_CONFIG.searchDebounceTime),
        distinctUntilChanged(),
        takeUntil(this.destroy$)
      )
      .subscribe((search) => {
        this.emitFilterChange();
      });
  }

  onStatusChange() {
    this.emitFilterChange();
  }

  onSortChange() {
    this.emitFilterChange();
  }

  onShowFinishedOrdersChange() {
    this.emitFilterChange();
  }

  clearFilters() {
    this.searchControl.setValue('');
    this.statusControl.setValue('');
    this.sortByControl.setValue('DateIn');
    this.showFinishedOrdersControl.setValue(false);
    this.emitFilterChange();
  }

  hasActiveFilters(): boolean {
    return !!(
      this.searchControl.value ||
      this.statusControl.value ||
      this.sortByControl.value !== 'DateIn' ||
      this.showFinishedOrdersControl.value
    );
  }

  private emitFilterChange() {
    const event: FilterChangeEvent = {
      search: this.searchControl.value || '',
      status: this.statusControl.value || '',
      sortBy: this.sortByControl.value || 'DateIn',
      showFinishedOrders: this.showFinishedOrdersControl.value || false,
    };

    this.filterChange.emit(event);
  }

  // Métodos públicos para controle externo
  setSearchValue(value: string) {
    this.searchControl.setValue(value, { emitEvent: false });
  }

  setStatusValue(value: OrderStatus | '') {
    this.statusControl.setValue(value, { emitEvent: false });
  }

  setSortByValue(value: string) {
    this.sortByControl.setValue(value, { emitEvent: false });
  }

  setShowFinishedOrdersValue(value: boolean) {
    this.showFinishedOrdersControl.setValue(value, { emitEvent: false });
  }

  // Método para resetar para valores padrão
  resetToDefaults() {
    this.searchControl.setValue('');
    this.statusControl.setValue('');
    this.sortByControl.setValue('DateIn');
    this.showFinishedOrdersControl.setValue(false);
  }
}