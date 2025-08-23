import { Component, OnInit, signal, inject, ChangeDetectionStrategy, DestroyRef, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { PaymentService } from '../../services/payment.service';
import { Payment, PaymentParams, Pagination } from '../../models/payment.interface';
import { debounceTime, Subject } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import {MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';


@Component({
  selector: 'app-payment-list',
  standalone: true,
  imports: [CommonModule, FormsModule, MatDatepickerModule, MatNativeDateModule],
  templateUrl: './payment-list.component.html',
  styleUrls: ['./payment-list.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class PaymentListComponent implements OnInit {
  private paymentService = inject(PaymentService);
  private router = inject(Router);
  private searchSubject = new Subject<string>();
  private readonly destroyRef = inject(DestroyRef);
 
  readonly defaultFilters: PaymentParams = {
    pageNumber: 1,
    pageSize: 10,
    search: '',
    startDate: '',
    endDate: ''
  };

  loading = signal(false);
  payments = signal<Payment[]>([]);
  pagination = signal<Pagination<Payment> | null>(null);
  filters = signal<PaymentParams>({ ...this.defaultFilters });

  ngOnInit(): void {
    this.setupSearchDebounce();
    this.loadPayments();
  }

  private setupSearchDebounce(): void {
    this.searchSubject.pipe(
      debounceTime(300),
      takeUntilDestroyed(this.destroyRef)
    ).subscribe(searchTerm => {
      this.filters.update(f => ({ ...f, search: searchTerm, pageNumber: 1 }));
      this.loadPayments();
    });
  }

  loadPayments(): void {
    this.loading.set(true);
    this.paymentService.getPayments(this.filters())
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (response) => {
          this.payments.set(response.data);
          this.pagination.set(response);
          this.loading.set(false);
        },
        error: (error) => {
            this.loading.set(false);
        }
      });
  }

  onFilterChange(): void {
    this.filters.update(f => ({ ...f, pageNumber: 1 }));
    this.loadPayments();
  }

  clearFilters(): void {
    this.filters.set({ ...this.defaultFilters });
    this.loadPayments();
  }

  changePage(delta: number): void {
    if (!this.pagination()) return;
    const newPage = this.pagination()!.pageNumber + delta;
    if (newPage >= 1 && newPage <= this.pagination()!.totalPages) {
      this.filters.update(f => ({ ...f, pageNumber: newPage }));
      this.loadPayments();
    }
  }

  navigateToCreate(): void {
    this.router.navigate(['/admin/payments/new']);
  }

  viewPayment(id: number): void {
    this.router.navigate(['/admin/payments', id]);
  }

  formatDate(dateString: string): string {
    return new Date(dateString).toLocaleDateString('pt-BR');
  }

  formatCurrency(value: number): string {
    return value.toLocaleString('pt-BR', { minimumFractionDigits: 2 });
  }

  setSearchFilter(value: string): void {
    this.searchSubject.next(value);
  }

  
  protected readonly paginationInfo = computed(() => {
    const p = this.pagination();
    if (!p) return null;
    
    return {
      start: (p.pageNumber - 1) * p.pageSize + 1,
      end: Math.min(p.pageNumber * p.pageSize, p.totalItems),
      total: p.totalItems
    };
  });

}
