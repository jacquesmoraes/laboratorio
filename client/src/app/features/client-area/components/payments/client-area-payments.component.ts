// client/src/app/features/client-area/components/payments/client-area-payments.component.ts
import { Component, OnInit, signal, inject, ChangeDetectionStrategy, input, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

import { ClientAreaService } from '../../services/client-area.service';
import { ClientPayment } from '../../models/client-area.interface';

type PaymentMonth = {
  monthKey: string;
  monthLabel: string;
  payments: ClientPayment[];
  total: number;
};

@Component({
  selector: 'app-client-area-payments',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './client-area-payments.component.html',
  styleUrls: ['./client-area-payments.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ClientAreaPaymentsComponent implements OnInit {
  private readonly clientAreaService = inject(ClientAreaService);

  // Inputs para reutilização
  limit = input<number>();
  showPagination = input<boolean>(true);
  showHeader = input<boolean>(true);
  showViewAllLink = input<boolean>(true);
  showMonthTabs = input<boolean>(true);

  paymentMonths = signal<PaymentMonth[]>([]);
  activeMonthKey = signal<string>('');
  isLoading = signal(true);

  ngOnInit() {
    this.loadPayments();
  }

  private loadPayments() {
    this.isLoading.set(true);

    this.clientAreaService.getPayments({ 
      pageNumber: 1, 
      pageSize: this.limit() || 100 
    }).subscribe({
      next: (response) => {
        const months = this.groupPaymentsByMonth(response.data);
        this.paymentMonths.set(months);

        if (months.length > 0) {
          this.activeMonthKey.set(months[0].monthKey);
        }

        this.isLoading.set(false);
      },
      error: (err) => {
        console.error('Erro ao carregar pagamentos', err);
        this.isLoading.set(false);
      }
    });
  }

  private groupPaymentsByMonth(payments: ClientPayment[]): PaymentMonth[] {
    const monthsMap = new Map<string, PaymentMonth>();

    // Agrupa pagamentos existentes por mês
    payments.forEach(payment => {
      const date = new Date(payment.paymentDate);
      const key = `${date.getFullYear()}-${(date.getMonth() + 1).toString().padStart(2, '0')}`;
      const label = date.toLocaleDateString('pt-BR', { month: 'long', year: 'numeric' });

      if (!monthsMap.has(key)) {
        monthsMap.set(key, { monthKey: key, monthLabel: label, payments: [], total: 0 });
      }

      const month = monthsMap.get(key)!;
      month.payments.push(payment);
      month.total += payment.amountPaid;
    });

    // Preenche meses vazios do ano atual (últimos 6 meses)
    const now = new Date();
    const currentYear = now.getFullYear();
    const currentMonth = now.getMonth();

    for (let i = 5; i >= 0; i--) {
      const monthIndex = currentMonth - i;
      const year = monthIndex < 0 ? currentYear - 1 : currentYear;
      const month = monthIndex < 0 ? monthIndex + 12 : monthIndex;
      
      const key = `${year}-${(month + 1).toString().padStart(2, '0')}`;
      const date = new Date(year, month, 1);
      const label = date.toLocaleDateString('pt-BR', { month: 'long', year: 'numeric' });

      if (!monthsMap.has(key)) {
        monthsMap.set(key, { monthKey: key, monthLabel: label, payments: [], total: 0 });
      }
    }

    return Array.from(monthsMap.values()).sort((a, b) => a.monthKey.localeCompare(b.monthKey));
  }

  setActiveMonth(monthKey: string) {
    this.activeMonthKey.set(monthKey);
  }

  getActiveMonthPayments(): ClientPayment[] {
    const month = this.paymentMonths().find(m => m.monthKey === this.activeMonthKey());
    return month?.payments || [];
  }

  getActiveMonthTotal(): number {
    const month = this.paymentMonths().find(m => m.monthKey === this.activeMonthKey());
    return month?.total || 0;
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(value);
  }

  formatDate(date: string): string {
    return new Date(date).toLocaleDateString('pt-BR');
  }

 // client/src/app/features/client-area/components/payments/client-area-payments.component.ts
getStatusClass(status: string | null | undefined): string {
  if (!status) return 'pending';
  
  switch (status.toLowerCase()) {
    case 'completed':
    case 'concluído':
    case 'paid':
    case 'pago':
      return 'completed';
    case 'pending':
    case 'pendente':
      return 'pending';
    case 'failed':
    case 'falhou':
      return 'failed';
    case 'cancelled':
    case 'cancelado':
      return 'cancelled';
    default:
      return 'pending';
  }
}

  activeMonthLabel = computed(() => {
  const month = this.paymentMonths().find(m => m.monthKey === this.activeMonthKey());
  return month?.monthLabel || '';
});
}