// client/src/app/features/client-area/client-area.routes.ts
import { Routes } from '@angular/router';
import { ClientAreaDashboardComponent } from './components/dashborad/client-area-dashboard.component';
import { ClientAreaInvoicesComponent } from './components/invoices/client-area-invoices.component';
import { ClientAreaOrdersComponent } from './components/orders/client-area-orders.component';
import { ClientAreaPaymentsComponent } from './components/payments/client-area-payments.component';

export const CLIENT_AREA_ROUTES: Routes = [
//   {
//     path: 'login',
//     component: ClientAreaLoginComponent
//   },
  {
    path: 'dashboard',
    component: ClientAreaDashboardComponent,
    // canActivate: [ClientAreaAuthGuard]
  },
  {
    path: 'invoices',
    component: ClientAreaInvoicesComponent,
    // canActivate: [ClientAreaAuthGuard]
  },
  {
    path: 'payments',
    component: ClientAreaPaymentsComponent,
    // canActivate: [ClientAreaAuthGuard]
  },
  {
    path: 'orders',
    component: ClientAreaOrdersComponent,
    // canActivate: [ClientAreaAuthGuard]
  },
  {
    path: '',
    redirectTo: 'dashboard',
    pathMatch: 'full'
  }
];