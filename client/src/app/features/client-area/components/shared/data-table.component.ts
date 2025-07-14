import { Component, input, output, signal, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TableColumn, PaginationInfo } from '../../models/client-area.interface';

@Component({
  selector: 'app-data-table',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="data-table-container">
      <!-- Filtros -->
      @if (showFilters()) {
        <div class="filters-section">
          <div class="search-box">
            <input 
              type="text" 
              placeholder="Buscar..."
              [ngModel]="searchTerm()"
              (ngModelChange)="onSearchChange($event)"
              class="search-input"
            />
          </div>
          
          <div class="filter-controls">
            @if (showStatusFilter()) {
              <select 
                [ngModel]="selectedStatus()"
                (ngModelChange)="onStatusChange($event)"
                class="filter-select"
              >
                <option value="">Todos os status</option>
                <option value="pending">Pendente</option>
                <option value="paid">Pago</option>
                <option value="overdue">Vencido</option>
              </select>
            }

            @if (showDateFilters()) {
              <div class="date-filters">
                <input 
                  type="date" 
                  [ngModel]="startDate()"
                  (ngModelChange)="onStartDateChange($event)"
                  class="date-input"
                />
                <span>até</span>
                <input 
                  type="date" 
                  [ngModel]="endDate()"
                  (ngModelChange)="onEndDateChange($event)"
                  class="date-input"
                />
              </div>
            }
          </div>
        </div>
      }

      <!-- Tabela -->
      <div class="table-container">
        <table class="data-table">
          <thead>
            <tr>
              @for (column of columns(); track column.key) {
                <th 
                  [style.width]="column.width"
                  [class.sortable]="column.sortable"
                  (click)="column.sortable ? onSort(column.key) : null"
                >
                  {{ column.label }}
                  @if (column.sortable && sortColumn() === column.key) {
                    <span class="sort-icon">
                      {{ sortDirection() === 'asc' ? '↑' : '↓' }}
                    </span>
                  }
                </th>
              }
            </tr>
          </thead>
          <tbody>
            @for (item of data(); track getItemKey(item)) {
              <tr>
                @for (column of columns(); track column.key) {
                  <td>
                    <ng-container [ngSwitch]="column.key">
                      @switch (column.key) {
                        @case ('totalServiceOrdersAmount') {
                          {{ formatCurrency(getItemValue(item, column.key)) }}
                        }
                        @case ('totalPaid') {
                          {{ formatCurrency(getItemValue(item, column.key)) }}
                        }
                        @case ('outstandingBalance') {
                          {{ formatCurrency(getItemValue(item, column.key)) }}
                        }
                        @case ('amountPaid') {
                          {{ formatCurrency(getItemValue(item, column.key)) }}
                        }
                        @case ('totalAmount') {
                          {{ formatCurrency(getItemValue(item, column.key)) }}
                        }
                        @case ('createdAt') {
                          {{ formatDate(getItemValue(item, column.key)) }}
                        }
                        @case ('paymentDate') {
                          {{ formatDate(getItemValue(item, column.key)) }}
                        }
                        @case ('dateIn') {
                          {{ formatDate(getItemValue(item, column.key)) }}
                        }
                        @case ('lastMovementDate') {
                          {{ formatDate(getItemValue(item, column.key)) }}
                        }
                        @case ('status') {
                          <span class="status-badge" [class]="getItemValue(item, column.key)">
                            {{ getItemValue(item, column.key) }}
                          </span>
                        }
                        @default {
                          {{ getItemValue(item, column.key) }}
                        }
                      }
                    </ng-container>
                  </td>
                }
              </tr>
            }
          </tbody>
        </table>
      </div>

      <!-- Paginação -->
      @if (showPagination() && paginationInfo()) {
        <div class="pagination-section">
          <div class="pagination-info">
            Mostrando {{ (paginationInfo()!.pageNumber - 1) * paginationInfo()!.pageSize + 1 }} 
            a {{ Math.min(paginationInfo()!.pageNumber * paginationInfo()!.pageSize, paginationInfo()!.totalItems) }} 
            de {{ paginationInfo()!.totalItems }} registros
          </div>
          
          <div class="pagination-controls">
            <button 
              (click)="onPageChange(paginationInfo()!.pageNumber - 1)"
              [disabled]="paginationInfo()!.pageNumber <= 1"
              class="pagination-btn"
            >
              Anterior
            </button>
            
            @for (page of getPageNumbers(); track page) {
              <button 
                (click)="onPageChange(page)"
                [class.active]="page === paginationInfo()!.pageNumber"
                class="pagination-btn"
              >
                {{ page }}
              </button>
            }
            
            <button 
              (click)="onPageChange(paginationInfo()!.pageNumber + 1)"
              [disabled]="paginationInfo()!.pageNumber >= paginationInfo()!.totalPages"
              class="pagination-btn"
            >
              Próximo
            </button>
          </div>
        </div>
      }
    </div>
  `,
  styles: [`
    .data-table-container {
      background: white;
      border-radius: 8px;
      box-shadow: 0 2px 4px rgba(0,0,0,0.1);
      overflow: hidden;
    }

    .filters-section {
      padding: 1rem;
      border-bottom: 1px solid #e9ecef;
      display: flex;
      gap: 1rem;
      align-items: center;
      flex-wrap: wrap;
    }

    .search-input {
      padding: 0.5rem;
      border: 1px solid #ced4da;
      border-radius: 4px;
      min-width: 200px;
    }

    .filter-select {
      padding: 0.5rem;
      border: 1px solid #ced4da;
      border-radius: 4px;
    }

    .date-filters {
      display: flex;
      align-items: center;
      gap: 0.5rem;
    }

    .date-input {
      padding: 0.5rem;
      border: 1px solid #ced4da;
      border-radius: 4px;
    }

    .table-container {
      overflow-x: auto;
    }

    .data-table {
      width: 100%;
      border-collapse: collapse;
    }

    .data-table th {
      background-color: #f8f9fa;
      padding: 1rem;
      text-align: left;
      font-weight: 600;
      color: #334a52;
      border-bottom: 1px solid #e9ecef;
    }

    .data-table th.sortable {
      cursor: pointer;
      user-select: none;
    }

    .data-table th.sortable:hover {
      background-color: #e9ecef;
    }

    .sort-icon {
      margin-left: 0.5rem;
    }

    .data-table td {
      padding: 1rem;
      border-bottom: 1px solid #e9ecef;
      color: #334a52;
    }

    .status-badge {
      padding: 0.25rem 0.5rem;
      border-radius: 4px;
      font-size: 0.8rem;
      font-weight: 500;
    }

    .status-badge.pending {
      background-color: #fff3cd;
      color: #856404;
    }

    .status-badge.paid {
      background-color: #d4edda;
      color: #155724;
    }

    .status-badge.overdue {
      background-color: #f8d7da;
      color: #721c24;
    }

    .pagination-section {
      padding: 1rem;
      display: flex;
      justify-content: space-between;
      align-items: center;
      border-top: 1px solid #e9ecef;
    }

    .pagination-controls {
      display: flex;
      gap: 0.5rem;
    }

    .pagination-btn {
      padding: 0.5rem 1rem;
      border: 1px solid #ced4da;
      background: white;
      border-radius: 4px;
      cursor: pointer;
      transition: all 0.2s;
    }

    .pagination-btn:hover:not(:disabled) {
      background-color: #e9ecef;
    }

    .pagination-btn.active {
      background-color: #276678;
      color: white;
      border-color: #276678;
    }

    .pagination-btn:disabled {
      opacity: 0.5;
      cursor: not-allowed;
    }

    @media (max-width: 768px) {
      .filters-section {
        flex-direction: column;
        align-items: stretch;
      }
      
      .pagination-section {
        flex-direction: column;
        gap: 1rem;
      }
    }
  `],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class DataTableComponent {
  // Inputs
  columns = input.required<TableColumn[]>();
  data = input.required<any[]>();
  paginationInfo = input<PaginationInfo | null>(null);
  
  // Configurações
  showFilters = input(true);
  showPagination = input(true);
  showStatusFilter = input(false);
  showDateFilters = input(false);
  
  // Outputs
  pageChange = output<number>();
  searchChange = output<string>();
  statusChange = output<string>();
  startDateChange = output<string>();
  endDateChange = output<string>();
  sortChange = output<{column: string, direction: string}>();

  // Signals internos
  searchTerm = signal('');
  selectedStatus = signal('');
  startDate = signal('');
  endDate = signal('');
  sortColumn = signal('');
  sortDirection = signal<'asc' | 'desc'>('asc');

  protected readonly Math = Math;

  onPageChange(page: number) {
    this.pageChange.emit(page);
  }

  onSearchChange(term: string) {
    this.searchTerm.set(term);
    this.searchChange.emit(term);
  }

  onStatusChange(status: string) {
    this.selectedStatus.set(status);
    this.statusChange.emit(status);
  }

  onStartDateChange(date: string) {
    this.startDate.set(date);
    this.startDateChange.emit(date);
  }

  onEndDateChange(date: string) {
    this.endDate.set(date);
    this.endDateChange.emit(date);
  }

  onSort(column: string) {
    if (this.sortColumn() === column) {
      this.sortDirection.set(this.sortDirection() === 'asc' ? 'desc' : 'asc');
    } else {
      this.sortColumn.set(column);
      this.sortDirection.set('asc');
    }
    
    this.sortChange.emit({
      column,
      direction: this.sortDirection()
    });
  }

  getItemKey(item: any): string {
    return item.id?.toString() || JSON.stringify(item);
  }

  getItemValue(item: any, key: string): any {
    return item[key];
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('pt-BR', { 
      style: 'currency', 
      currency: 'BRL' 
    }).format(value || 0);
  }

  formatDate(value: string): string {
    if (!value) return '';
    return new Date(value).toLocaleDateString('pt-BR');
  }

  getPageNumbers(): number[] {
    const pagination = this.paginationInfo();
    if (!pagination) return [];

    const current = pagination.pageNumber;
    const total = pagination.totalPages;
    const pages: number[] = [];

    // Mostrar no máximo 5 páginas
    let start = Math.max(1, current - 2);
    let end = Math.min(total, current + 2);

    if (end - start < 4) {
      if (start === 1) {
        end = Math.min(total, start + 4);
      } else {
        start = Math.max(1, end - 4);
      }
    }

    for (let i = start; i <= end; i++) {
      pages.push(i);
    }

    return pages;
  }
}