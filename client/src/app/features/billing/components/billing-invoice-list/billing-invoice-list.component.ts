import { Component, signal, inject, OnInit, OnDestroy, DestroyRef, computed } from '@angular/core';
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
import { debounceTime, Subject } from 'rxjs';
import { MatTableModule } from '@angular/material/table';
import Swal from 'sweetalert2';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { MatChip, MatChipsModule } from "@angular/material/chips";

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
    MatChipsModule,
    MatProgressSpinnerModule,
    MatIconModule,
    MatPaginatorModule,
    MatCardModule,
    MatSelectModule,
    MatChip
],
  templateUrl: './billing-invoice-list.component.html',
  styleUrls: ['./billing-invoice-list.component.scss']
})
export class BillingInvoiceListComponent implements OnInit {
  private billingService = inject(BillingInvoiceService);
  private router = inject(Router);
  private refreshSubject = new Subject<void>();
  private readonly destroyRef = inject(DestroyRef);
  loading = signal(false);
  invoices = signal<BillingInvoice[]>([]);
  pagination = signal<Pagination<BillingInvoice> | null>(null);

  private readonly defaultFilters: InvoiceParams = {
    pageNumber: 1,
    pageSize: 10,
    search: '',
    startDate: '',
    endDate: ''
  };
  filters = signal<InvoiceParams>({ ...this.defaultFilters });

  

  displayedColumns = ['invoiceNumber', 'clientName', 'createdAt', 'status', 'total', 'actions'];

  ngOnInit(): void {
    // Debounce apenas para filtros de busca
    this.refreshSubject
      .pipe(
        debounceTime(300),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe(() => this.loadInvoices());
  
    this.loadInvoices();
  }

  loadInvoices(): void {
    this.loading.set(true);
    this.billingService.getInvoices(this.filters())
    .pipe(takeUntilDestroyed(this.destroyRef))
    .subscribe({
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

  updateFilter<K extends keyof InvoiceParams>(key: K, value: InvoiceParams[K]): void {
    this.filters.update(f => ({ ...f, [key]: value, pageNumber: 1 }));
    this.refreshSubject.next();
  }
  
  changePage(pageNumber: number): void {
    this.filters.update(f => ({ ...f, pageNumber }));
    this.loadInvoices(); 
  }
  clearFilters(): void {
    this.filters.set({ ...this.defaultFilters });
    this.refreshSubject.next();
  }

  navigateToCreate(): void {
    this.router.navigate(['/billing/new']);
  }

  viewInvoice(id: number): void {
    this.router.navigate(['/billing', id]);
  }

 cancelInvoice(id: number): void {
  Swal.fire({
    title: 'Cancelar fatura?',
    text: 'Esta ação não poderá ser desfeita.',
    icon: 'warning',
    showCancelButton: true,
    confirmButtonColor: '#d33',
    cancelButtonColor: '#3085d6',
    confirmButtonText: 'Sim, cancelar',
    cancelButtonText: 'Não'
  }).then(result => {
    if (result.isConfirmed) {
      this.billingService.cancelInvoice(id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          Swal.fire('Cancelada!', 'A fatura foi cancelada com sucesso.', 'success');
          this.loadInvoices(); // ou reload da fatura no caso de details
        },
        error: () => {
          Swal.fire('Erro!', 'Não foi possível cancelar a fatura.', 'error');
        }
      });
    }
  });
}

protected readonly paginationInfo = computed(() => {
  const p = this.pagination();
  if (!p) return null;
  
  return {
    currentPage: p.pageNumber,
    totalPages: p.totalPages,
    hasNext: p.pageNumber < p.totalPages,
    hasPrev: p.pageNumber > 1
  };
});

protected readonly getStatusClass = computed(() => {
  return (status: InvoiceStatus): string => {
    const statusClassMap: Record<InvoiceStatus, string> = {
      Open: 'status-open',
      PartiallyPaid: 'status-partially',
      Paid: 'status-paid',
      Cancelled: 'status-cancelled',
      Closed: 'status-closed'
    };
    return statusClassMap[status] ?? '';
  };
});

 
  
}
