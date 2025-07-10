import { Component, OnInit, signal, inject, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { PaymentService } from '../../services/payment.service';
import { Payment } from '../../models/payment.interface';

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

  loading = signal(false);
  payment = signal<Payment | null>(null);

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.loadPayment(parseInt(id));
    }
  }

  private loadPayment(id: number): void {
    this.loading.set(true);
    this.paymentService.getPaymentById(id).subscribe({
      next: (payment) => {
        this.payment.set(payment);
        this.loading.set(false);
      },
      error: (error) => {
        console.error('Erro ao carregar pagamento:', error);
        this.loading.set(false);
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/payments']);
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