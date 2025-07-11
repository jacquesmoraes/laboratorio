// client/src/app/features/client-area/components/invoices/client-area-invoices.component.ts
import { Component, OnInit, signal, inject, ChangeDetectionStrategy, input } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ClientAreaService } from '../../services/client-area.service';
import { ClientInvoice, InvoiceParams } from '../../models/client-area.interface';

@Component({
  selector: 'app-client-area-invoices',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './client-area-invoices.component.html',
  styleUrls: ['./client-area-invoices.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ClientAreaInvoicesComponent implements OnInit {
  private readonly clientAreaService = inject(ClientAreaService);

  // Inputs para reutilização
  limit = input<number>();
  showPagination = input<boolean>(true);
  showHeader = input<boolean>(true);
  showViewAllLink = input<boolean>(true);

  invoices = signal<ClientInvoice[]>([]);
  isLoading = signal(true);
  currentPage = signal(1);
  totalPages = signal(1);
  totalInvoices = signal(0);
  itemsPerPage = 10;

  ngOnInit() {
    this.loadInvoices();
  }

  loadInvoices(page: number = 1) {
    this.isLoading.set(true);

    const params: InvoiceParams = {
      pageNumber: page,
      pageSize: this.limit() || this.itemsPerPage
    };

    this.clientAreaService.getInvoices(params).subscribe({
      next: (res) => {
        this.invoices.set(res.data);
        this.currentPage.set(res.pageNumber);
        this.totalPages.set(res.totalPages);
        this.totalInvoices.set(res.totalItems);
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error('Erro ao carregar faturas', err);
        this.isLoading.set(false);
      }
    });
  }

  onPageChange(page: number) {
    this.loadInvoices(page);
  }

  downloadInvoice(invoice: ClientInvoice) {
    this.clientAreaService.downloadInvoice(invoice.billingInvoiceId).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = `fatura-${invoice.invoiceNumber}.pdf`;
        link.click();
        window.URL.revokeObjectURL(url);
      },
      error: (err) => {
        console.error('Erro ao baixar fatura', err);
      }
    });
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(value);
  }

  formatDate(date: string): string {
    return new Date(date).toLocaleDateString('pt-BR');
  }
}