import { Routes } from '@angular/router';
import { ClientAreaLayoutComponent } from './components/layout/client-area-layout.component';

import { ClientAreaInvoicesComponent } from './components/invoices/client-area-invoices.component';
import { ClientAreaOrdersComponent } from './components/orders/client-area-orders.component';
import { ClientAreaPaymentsComponent } from './components/payments/client-area-payments.component';
import { ClientAreaDashboardComponent } from './components/dashborad/client-area-dashboard.component';

export const CLIENT_AREA_ROUTES: Routes = [
  {
    path: '',
    component: ClientAreaLayoutComponent,
    children: [
      {
        path: 'dashboard',
        component: ClientAreaDashboardComponent
      },
      {
        path: 'invoices',
        component: ClientAreaInvoicesComponent
      },
      {
        path: 'orders',
        component: ClientAreaOrdersComponent
      },
      {
        path: 'payments',
        component: ClientAreaPaymentsComponent
      },
      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full'
      }
    ]
  }
];