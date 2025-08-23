import { Component, OnInit, signal, inject, ChangeDetectionStrategy, DestroyRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { PaymentService } from '../../services/payment.service';
import { Payment } from '../../models/payment.interface';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-payment-details',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './payment-details.component.html',
  styleUrls: ['./payment-details.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class PaymentDetailsComponent implements OnInit {
  private paymentService = inject(PaymentService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);
  loading = signal(false);
  payment = signal<Payment | null>(null);

  
  get p(): Payment | null {
    return this.payment();
  }

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    const id = Number(idParam);
    if (!isNaN(id)) {
      this.loadPayment(id);
    } else {
      console.error('ID de pagamento inválido');
      this.goBack();
    }
  }

  private loadPayment(id: number): void {
    this.loading.set(true);
    this.paymentService.getPaymentById(id)
    .pipe(takeUntilDestroyed(this.destroyRef))
    .subscribe({
      next: (payment) => {
        this.payment.set(payment);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/admin/payments']);
  }

  formatDate(dateString: string): string {
    return new Date(dateString).toLocaleDateString('pt-BR', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  formatCurrency(value: number): string {
    return value.toLocaleString('pt-BR', { minimumFractionDigits: 2 });
  }
}
