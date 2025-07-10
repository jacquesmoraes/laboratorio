import { Component, OnInit, signal, computed, inject, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { PaymentService } from '../../services/payment.service';
import { Payment, PaymentParams } from '../../models/payment.interface';

@Component({
  selector: 'app-payment-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './payment-list.component.html',
  styleUrls: ['./payment-list.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class PaymentListComponent implements OnInit {
  private paymentService = inject(PaymentService);
  private router = inject(Router);

  loading = signal(false);
  payments = signal<Payment[]>([]);
  pagination = signal<any>(null);
  filters = signal<PaymentParams>({
    pageNumber: 1,
    pageSize: 10,
    search: '',
    startDate: '',
    endDate: ''
  });

  // Computed signals for form binding
  searchFilter = computed(() => this.filters().search);
  startDateFilter = computed(() => this.filters().startDate);
  endDateFilter = computed(() => this.filters().endDate);

  Math = Math;

  ngOnInit(): void {
    this.loadPayments();
  }

  loadPayments(): void {
    this.loading.set(true);
    this.paymentService.getPayments(this.filters()).subscribe({
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

  onFilterChange(): void {
    this.filters.update(f => ({ ...f, pageNumber: 1 }));
    this.loadPayments();
  }

  clearFilters(): void {
    this.filters.set({
      pageNumber: 1,
      pageSize: 10,
      search: '',
      startDate: '',
      endDate: ''
    });
    this.loadPayments();
  }

  previousPage(): void {
    if (this.pagination() && this.pagination().pageNumber > 1) {
      this.filters.update(f => ({ ...f, pageNumber: f.pageNumber - 1 }));
      this.loadPayments();
    }
  }

  nextPage(): void {
    if (this.pagination() && this.pagination().pageNumber < this.pagination().totalPages) {
      this.filters.update(f => ({ ...f, pageNumber: f.pageNumber + 1 }));
      this.loadPayments();
    }
  }

  navigateToCreate(): void {
    this.router.navigate(['/payments/new']);
  }

  viewPayment(id: number): void {
    this.router.navigate(['/payments', id]);
  }

  formatDate(dateString: string): string {
    return new Date(dateString).toLocaleDateString('pt-BR');
  }

  formatCurrency(value: number): string {
    return value.toLocaleString('pt-BR', { minimumFractionDigits: 2 });
  }

  // Setters for form binding
  setSearchFilter(value: string): void {
    this.filters.update(f => ({ ...f, search: value }));
  }

  setStartDateFilter(value: string): void {
    this.filters.update(f => ({ ...f, startDate: value }));
  }

  setEndDateFilter(value: string): void {
    this.filters.update(f => ({ ...f, endDate: value }));
  }
}