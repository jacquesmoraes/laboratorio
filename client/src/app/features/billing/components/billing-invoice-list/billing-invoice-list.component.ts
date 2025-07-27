import { Component, signal, inject, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { BillingInvoiceService } from '../../services/billing-invoice.service';
import { BillingInvoice, InvoiceParams, Pagination, InvoiceStatus } from '../../models/billing-invoice.interface';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatNativeDateModule } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { MatTableModule } from '@angular/material/table';
import { debounceTime, Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-billing-invoice-list',
  standalone: true,
  imports: [
      CommonModule,
  FormsModule,
  MatButtonModule,
  MatFormFieldModule,
  MatInputModule,
  MatDatepickerModule,
  MatNativeDateModule,
  MatTableModule,
  MatProgressSpinnerModule,
  MatIconModule,
  MatPaginatorModule,
  MatCardModule,
  MatSelectModule
    ],
  templateUrl: './billing-invoice-list.component.html',
  styleUrls: ['./billing-invoice-list.component.scss']
})
export class BillingInvoiceListComponent implements OnInit, OnDestroy  {
  private billingService = inject(BillingInvoiceService);
  private router = inject(Router);
  private destroy$ = new Subject<void>();
  private searchSubject = new Subject<string>();
  loading = signal(false);
  invoices = signal<BillingInvoice[]>([]);
  pagination = signal<Pagination<BillingInvoice> | null>(null);
  filters = signal<InvoiceParams>(this.resetFilters());
displayedColumns = ['invoiceNumber', 'clientName', 'createdAt', 'status', 'total', 'actions'];

  ngOnInit(): void {
    this.setupSearchDebounce();
    this.loadInvoices();
  }

  loadInvoices(): void {
    this.loading.set(true);
    this.billingService.getInvoices(this.filters()).subscribe({
      next: (response) => {
        this.invoices.set(response.data);
        this.pagination.set(response);
        this.loading.set(false);
      },
      error: (error) => {
        console.error('Erro ao carregar faturas:', error);
        this.loading.set(false);
      }
    });
  }

  onSearchChange(value: string): void {
    this.searchSubject.next(value);
  }

private setupSearchDebounce(): void {
  this.searchSubject.pipe(
    debounceTime(500),
    takeUntil(this.destroy$)
  ).subscribe(searchTerm => {
    this.filters.update(f => ({
      ...f,
      pageNumber: 1,
      search: searchTerm
    }));
    this.loadInvoices();
  });
}


  onPageChange(page: number): void {
    this.filters.update(f => ({ ...f, pageNumber: page }));
    this.loadInvoices();
  }

  clearFilters(): void {
    this.filters.set(this.resetFilters());
    this.loadInvoices();
  }

  resetFilters(): InvoiceParams {
    return {
      pageNumber: 1,
      pageSize: 10,
      search: '',
      startDate: '',
      endDate: ''
    };
  }

  navigateToCreate(): void {
    this.router.navigate(['/billing/new']);
  }

  viewInvoice(id: number): void {
    this.router.navigate(['/billing', id]);
  }

  cancelInvoice(id: number): void {
    if (confirm('Tem certeza que deseja cancelar esta fatura?')) {
      this.billingService.cancelInvoice(id).subscribe({
        next: () => {
          this.loadInvoices();
        },
        error: (error) => {
          console.error('Erro ao cancelar fatura:', error);
          alert('Erro ao cancelar fatura');
        }
      });
    }
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

  getStatusClass(status: InvoiceStatus): string {
    switch (status) {
      case 'Open':
        return 'status-open';
      case 'PartiallyPaid':
        return 'status-partially';
      case 'Paid':
        return 'status-paid';
      case 'Cancelled':
        return 'status-cancelled';
      case 'Closed':
        return 'status-closed';
      default:
        return '';
    }
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
