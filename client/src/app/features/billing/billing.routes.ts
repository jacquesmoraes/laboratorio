import { Routes } from '@angular/router';

export const BILLING_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./components/billing-invoice-list/billing-invoice-list.component')
      .then(m => m.BillingInvoiceListComponent),
    title: 'Faturas'
  },
  {
    path: 'new',
    loadComponent: () => import('./components/billing-invoice-create/billing-invoice-create.component')
      .then(m => m.BillingInvoiceCreateComponent),
    title: 'Nova Fatura'
  },
  {
    path: ':id',
    loadComponent: () => import('./components/billing-invoice-details/billing-invoice-details.component')
      .then(m => m.BillingInvoiceDetailsComponent),
    title: 'Detalhes da Fatura'
  }
];