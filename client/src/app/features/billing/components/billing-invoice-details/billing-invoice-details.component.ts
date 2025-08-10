import { Component, signal, inject, OnInit, OnDestroy, DestroyRef, computed } from '@angular/core';
import { CommonModule, DatePipe, CurrencyPipe } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { BillingInvoiceService } from '../../services/billing-invoice.service';
import { BillingInvoice } from '../../models/billing-invoice.interface';
import Swal from 'sweetalert2';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-billing-invoice-details',
  standalone: true,
  imports: [CommonModule, DatePipe, CurrencyPipe],
  templateUrl: './billing-invoice-details.component.html',
  styleUrls: ['./billing-invoice-details.component.scss']
})
export class BillingInvoiceDetailsComponent implements OnInit {
  private billingService = inject(BillingInvoiceService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);

  loading = signal(false);
  invoice = signal<BillingInvoice | null>(null);

  ngOnInit(): void {
    this.route.paramMap
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(params => {
        const id = Number(params.get('id'));
        if (id) this.loadInvoice(id);
      });
  }

  loadInvoice(id: number): void {
    this.loading.set(true);
      this.billingService.getInvoiceById(id)
    .pipe(takeUntilDestroyed(this.destroyRef))
    .subscribe({
      next: invoice => {
        this.invoice.set(invoice);
        this.loading.set(false);
      },
      error: err => {
        console.error('Erro ao carregar fatura:', err);
        this.loading.set(false);
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/billing']);
  }

  printInvoice(): void {
    const inv = this.invoice();
    if (!inv) return;

    this.billingService.downloadInvoicePdf(inv.billingInvoiceId)
    .pipe(takeUntilDestroyed(this.destroyRef))
    .subscribe({
      next: blob => {
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = `fatura-${inv.invoiceNumber}.pdf`;
        link.click();
        window.URL.revokeObjectURL(url);
      },
      error: err => {
        console.error('Erro ao baixar PDF:', err);
        alert('Erro ao gerar PDF');
      }
    });
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
          this.loadInvoice(id); // ou reload da fatura no caso de details
        },
        error: () => {
          Swal.fire('Erro!', 'Não foi possível cancelar a fatura.', 'error');
        }
      });
    }
  });
}

protected readonly invoiceInfo = computed(() => {
  const inv = this.invoice();
  if (!inv) return null;
  
  return {
    id: inv.billingInvoiceId,
    number: inv.invoiceNumber,
    createdAt: inv.createdAt,
    description: inv.description,
    client: inv.client,
    serviceOrders: inv.serviceOrders,
    totalAmount: inv.totalInvoiceAmount
  };
});


}
