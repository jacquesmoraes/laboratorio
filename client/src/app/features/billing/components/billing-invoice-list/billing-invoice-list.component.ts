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
import { debounceTime, Subject, takeUntil } from 'rxjs';
import { MatTableModule } from '@angular/material/table';
import Swal from 'sweetalert2';

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
export class BillingInvoiceListComponent implements OnInit, OnDestroy {
  private billingService = inject(BillingInvoiceService);
  private router = inject(Router);
  private destroy$ = new Subject<void>();
  private refreshSubject = new Subject<void>();

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

  private readonly statusClassMap: Record<InvoiceStatus, string> = {
    Open: 'status-open',
    PartiallyPaid: 'status-partially',
    Paid: 'status-paid',
    Cancelled: 'status-cancelled',
    Closed: 'status-closed'
  };

  displayedColumns = ['invoiceNumber', 'clientName', 'createdAt', 'status', 'total', 'actions'];

  ngOnInit(): void {
    this.refreshSubject
      .pipe(debounceTime(300), takeUntil(this.destroy$))
      .subscribe(() => this.loadInvoices());

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

  updateFilter<K extends keyof InvoiceParams>(key: K, value: InvoiceParams[K]): void {
    this.filters.update(f => ({ ...f, [key]: value, pageNumber: 1 }));
    this.refreshSubject.next();
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
      this.billingService.cancelInvoice(id).subscribe({
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

  getStatusClass(status: InvoiceStatus): string {
    return this.statusClassMap[status] ?? '';
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
