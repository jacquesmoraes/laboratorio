import { Component, input, signal, computed, inject, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { Subject, debounceTime, takeUntil } from 'rxjs';
import { PaymentService } from '../../../payments/services/payment.service';
import { Payment, PaymentParams, Pagination } from '../../../payments/models/payment.interface';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-client-payments',
  standalone: true,
  imports: [CommonModule, MatIconModule],
  templateUrl: './client-payments.component.html',
  styleUrls: ['./client-payments.component.scss']
})
export class ClientPaymentsComponent implements OnInit, OnDestroy {
  private paymentService = inject(PaymentService);
  private router = inject(Router);
  private destroy$ = new Subject<void>();
  private searchSubject = new Subject<string>();
Math = Math;
  // Input para receber o clientId do componente pai
  clientId = input.required<number>();

  loading = signal(false);
  payments = signal<Payment[]>([]);
  pagination = signal<Pagination<Payment> | null>(null);
  
  // ✅ CORREÇÃO: Usar computed para garantir que clientId esteja disponível
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

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private setupSearchDebounce(): void {
    this.searchSubject.pipe(
      debounceTime(500),
      takeUntil(this.destroy$)
    ).subscribe(searchTerm => {
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

    this.paymentService.getPayments(params).subscribe({
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

  onSearchChange(value: string): void {
    this.searchSubject.next(value);
  }

  onPageChange(page: number): void {
    const currentFilters = this.filters();
    const params: PaymentParams = {
      ...currentFilters,
      pageNumber: page
    };
    
    this.loading.set(true);
    this.paymentService.getPayments(params).subscribe({
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
    this.loadPayments('');
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