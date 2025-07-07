import { Routes } from '@angular/router';
import { ServiceOrderFormComponent } from './components/service-order-form/service-order-form.component';
import { ServiceOrderListComponent } from './components/service-order-list/service-order-list.component';
import { ServiceOrderDetailsComponent } from './components/service-order-details/service-order-details.component';

export const SERVICE_ORDERS_ROUTES: Routes = [
  {
    path: '',
    component: ServiceOrderListComponent
  },
  {
    path: 'new',
    component: ServiceOrderFormComponent
  },
  {
    path: ':id',
    component: ServiceOrderDetailsComponent
  },
  {
    path: ':id/edit',
    component: ServiceOrderFormComponent
  }
];