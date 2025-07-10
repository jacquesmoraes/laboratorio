import { Routes } from '@angular/router';

export const PAYMENT_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./components/payment-list/payment-list.component')
      .then(m => m.PaymentListComponent),
    title: 'Pagamentos'
  },
  {
    path: 'new',
    loadComponent: () => import('./components/payment-create/payment-create.component')
      .then(m => m.PaymentCreateComponent),
    title: 'Novo Pagamento'
  },
  {
    path: ':id',
    loadComponent: () => import('./components/payment-details/payment-details.component')
      .then(m => m.PaymentDetailsComponent),
    title: 'Detalhes do Pagamento'
  }
];