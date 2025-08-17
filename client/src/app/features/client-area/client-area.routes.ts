import { Routes } from '@angular/router';

import { ClientAreaLayoutComponent } from './client-area-layout.component';
import { ClientAreaDashboardComponent } from './components/client-area-dashboard.component';
import { ClientAreaInvoicesComponent } from './components/client-area-invoices.component';
import { ClientAreaOrdersComponent } from './components/client-area-orders.component';
import { ClientAreaPaymentsComponent } from './components/client-area-payments.component';
import { ClientAreaOrderDetailsComponent } from './components/client-area-order-details.component';

export const CLIENT_AREA_ROUTES: Routes = [
  {
    path: '',
    component: ClientAreaLayoutComponent,
    children: [
      { path: '', component: ClientAreaDashboardComponent,title: 'Dashboard' },
      { path: 'payments', component: ClientAreaPaymentsComponent, title: 'Pagamentos' },
      { path: 'invoices', component: ClientAreaInvoicesComponent, title: 'Faturas' },
      { path: 'orders', component: ClientAreaOrdersComponent, title: 'Pedidos' },
      { path: 'orders/:id', component: ClientAreaOrderDetailsComponent, title: 'Detalhes do Pedido' },
{ 
        path: 'change-password', 
        loadComponent: () => import('../../core/auth/change-password/change-password.component').then(m => m.ChangePasswordComponent) 
      }
    ]
  }
];
