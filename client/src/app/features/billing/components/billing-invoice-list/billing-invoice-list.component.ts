import { Component, signal, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { BillingInvoiceService } from '../../services/billing-invoice.service';
import { BillingInvoice, InvoiceParams, Pagination, InvoiceStatus } from '../../models/billing-invoice.interface';

@Component({
  selector: 'app-billing-invoice-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './billing-invoice-list.component.html',
  styleUrls: ['./billing-invoice-list.component.scss']
})
export class BillingInvoiceListComponent implements OnInit {
  private billingService = inject(BillingInvoiceService);
  private router = inject(Router);

  loading = signal(false);
  invoices = signal<BillingInvoice[]>([]);
  pagination = signal<Pagination<BillingInvoice> | null>(null);
  filters = signal<InvoiceParams>(this.resetFilters());

  ngOnInit(): void {
    this.loadInvoices();
  }

  loadInvoices(): void {
    this.loading.set(true);
    this.billingService.getInvoices(this.filters()).subscribe({
      next: (response) => {
        this.invoices.set(response.data);
        this.pagination.set(response);
        this.loading.set(false);
      },
      error: (error) => {
        console.error('Erro ao carregar faturas:', error);
        this.loading.set(false);
      }
    });
  }

  onSearchChange(value: string): void {
  this.filters.update(f => ({
    ...f,
    pageNumber: 1,
    search: value
  }));
  this.loadInvoices();
}


  onPageChange(page: number): void {
    this.filters.update(f => ({ ...f, pageNumber: page }));
    this.loadInvoices();
  }

  clearFilters(): void {
    this.filters.set(this.resetFilters());
    this.loadInvoices();
  }

  resetFilters(): InvoiceParams {
    return {
      pageNumber: 1,
      pageSize: 10,
      search: '',
      startDate: '',
      endDate: ''
    };
  }

  navigateToCreate(): void {
    this.router.navigate(['/billing/new']);
  }

  viewInvoice(id: number): void {
    this.router.navigate(['/billing', id]);
  }

  cancelInvoice(id: number): void {
    if (confirm('Tem certeza que deseja cancelar esta fatura?')) {
      this.billingService.cancelInvoice(id).subscribe({
        next: () => {
          this.loadInvoices();
        },
        error: (error) => {
          console.error('Erro ao cancelar fatura:', error);
          alert('Erro ao cancelar fatura');
        }
      });
    }
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

  getStatusClass(status: InvoiceStatus): string {
    switch (status) {
      case 'Open':
        return 'status-open';
      case 'PartiallyPaid':
        return 'status-partially';
      case 'Paid':
        return 'status-paid';
      case 'Cancelled':
        return 'status-cancelled';
      case 'Closed':
        return 'status-closed';
      default:
        return '';
    }
  }
}
