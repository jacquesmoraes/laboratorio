import { Component, input, signal, computed, inject, OnInit, OnDestroy, DestroyRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { Subject, debounceTime } from 'rxjs';
import { PaymentService } from '../../../payments/services/payment.service';
import { Payment, PaymentParams, Pagination } from '../../../payments/models/payment.interface';
import { MatIconModule } from '@angular/material/icon';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormControl, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-client-payments',
  standalone: true,
  imports: [CommonModule, MatIconModule, ReactiveFormsModule],
  templateUrl: './client-payments.component.html',
  styleUrls: ['./client-payments.component.scss']
})
export class ClientPaymentsComponent implements OnInit {
  private paymentService = inject(PaymentService);
  private router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);
  searchControl = new FormControl<string>('', { nonNullable: true });

  // Input para receber o clientId do componente pai
  clientId = input.required<number>();

  loading = signal(false);
  payments = signal<Payment[]>([]);
  pagination = signal<Pagination<Payment> | null>(null);

  filters = computed(() => ({
    pageNumber: 1,
    pageSize: 5,
    clientId: this.clientId(),
    search: '',
    startDate: '',
    endDate: ''
  } as PaymentParams));

  ngOnInit(): void {
    this.setupSearchDebounce();
    this.loadPayments();
  }

  private setupSearchDebounce(): void {
    this.searchControl.valueChanges
      .pipe(
        debounceTime(300),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe(searchTerm => {
        this.loadPayments(searchTerm);
      });
  }

  private loadPayments(searchTerm?: string): void {
    this.loading.set(true);

    const currentFilters = this.filters();
    const params: PaymentParams = {
      ...currentFilters,
      search: searchTerm || currentFilters.search
    };

    this.paymentService.getPayments(params)
    .pipe(takeUntilDestroyed(this.destroyRef))
    .subscribe({
      next: (response) => {
        this.payments.set(response.data);
        this.pagination.set(response);
        this.loading.set(false);
      },
      error: (error) => {
        console.error('Erro ao carregar pagamentos:', error);
        this.loading.set(false);
      }
    });
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


  onSearchChange(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.searchControl.setValue(input.value);
  }

  onPageChange(page: number): void {
    const currentFilters = this.filters();
    const params: PaymentParams = {
      ...currentFilters,
      pageNumber: page
    };

    this.loading.set(true);
    this.paymentService.getPayments(params)
    .pipe(takeUntilDestroyed(this.destroyRef))
    .subscribe({
      next: (response) => {
        this.payments.set(response.data);
        this.pagination.set(response);
        this.loading.set(false);
      },
      error: (error) => {
        console.error('Erro ao carregar pagamentos:', error);
        this.loading.set(false);
      }
    });
  }

  clearFilters(): void {
    this.searchControl.setValue('');
  }

  viewPayment(id: number): void {
    this.router.navigate(['/payments', id]);
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
}
