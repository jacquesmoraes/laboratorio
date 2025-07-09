import { Component, signal, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { BillingInvoiceService } from '../../services/billing-invoice.service';
import { BillingInvoice } from '../../models/billing-invoice.interface';

@Component({
  selector: 'app-billing-invoice-details',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './billing-invoice-details.component.html',
  styleUrls: ['./billing-invoice-details.component.scss']
})
export class BillingInvoiceDetailsComponent implements OnInit {
  private billingService = inject(BillingInvoiceService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  loading = signal(false);
  invoice = signal<BillingInvoice | null>(null);

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.loadInvoice(parseInt(id));
    }
  }

  loadInvoice(id: number): void {
    this.loading.set(true);
    this.billingService.getInvoiceById(id).subscribe({
      next: (invoice) => {
        this.invoice.set(invoice);
        this.loading.set(false);
      },
      error: (error) => {
        console.error('Erro ao carregar fatura:', error);
        this.loading.set(false);
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/billing']);
  }

  printInvoice(): void {
    if (this.invoice()) {
      this.billingService.downloadInvoicePdf(this.invoice()!.billingInvoiceId).subscribe({
        next: (blob) => {
          const url = window.URL.createObjectURL(blob);
          const link = document.createElement('a');
          link.href = url;
          link.download = `fatura-${this.invoice()!.invoiceNumber}.pdf`;
          link.click();
          window.URL.revokeObjectURL(url);
        },
        error: (error) => {
          console.error('Erro ao baixar PDF:', error);
          alert('Erro ao gerar PDF');
        }
      });
    }
  }

  cancelInvoice(): void {
    if (this.invoice() && confirm('Tem certeza que deseja cancelar esta fatura?')) {
      this.billingService.cancelInvoice(this.invoice()!.billingInvoiceId).subscribe({
        next: () => {
          this.loadInvoice(this.invoice()!.billingInvoiceId);
        },
        error: (error) => {
          console.error('Erro ao cancelar fatura:', error);
          alert('Erro ao cancelar fatura');
        }
      });
    }
  }

  formatDate(date?: string): string {
    if (!date) return '';
    return new Date(date).toLocaleDateString('pt-BR');
  }

  formatCurrency(value?: number): string {
    if (value === undefined || value === null) return 'R$ 0,00';
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL'
    }).format(value);
  }
}