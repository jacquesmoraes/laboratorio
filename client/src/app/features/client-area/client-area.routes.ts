import { Routes } from '@angular/router';

import { ClientAreaLayoutComponent } from './client-area-layout.component';
import { ClientAreaDashboardComponent } from './components/client-area-dashboard.component';
import { ClientAreaInvoicesComponent } from './components/client-area-invoices.component';
import { ClientAreaOrdersComponent } from './components/client-area-orders.component';
import { ClientAreaPaymentsComponent } from './components/client-area-payments.component';

export const CLIENT_AREA_ROUTES: Routes = [
  {
    path: '',
    component: ClientAreaLayoutComponent,
    children: [
      { path: '', component: ClientAreaDashboardComponent },
      { path: 'payments', component: ClientAreaPaymentsComponent },
      { path: 'invoices', component: ClientAreaInvoicesComponent },
      { path: 'orders', component: ClientAreaOrdersComponent }
    ]
  }
];
