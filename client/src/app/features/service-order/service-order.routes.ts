import { Routes } from '@angular/router';
import { ServiceOrderFormComponent } from './components/service-order-form/service-order-form.component';
import { ServiceOrderListComponent } from './components/service-order-list/service-order-list.component';
import { ServiceOrderDetailsComponent } from './components/service-order-details/service-order-details.component';

export const SERVICE_ORDERS_ROUTES: Routes = [
  {
    path: '',
    component: ServiceOrderListComponent,
    title: 'Ordens de Serviço'
  },
  {
    path: 'new',
    component: ServiceOrderFormComponent,
    title: 'Nova Ordem de Serviço'
  },
  {
    path: ':id',
    component: ServiceOrderDetailsComponent,
    title: 'Detalhes da Ordem de Serviço'
  },
  {
    path: ':id/edit',
    component: ServiceOrderFormComponent,
    title: 'Editar Ordem de Serviço'
  }
];