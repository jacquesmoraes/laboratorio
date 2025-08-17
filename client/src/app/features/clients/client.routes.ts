import { Routes } from '@angular/router';
import { ClientFormComponent } from './components/clients-form/client-form.component';
import { ClientListComponent } from './components/clients-list/client-list.component';
import { ClientDetailsComponent } from './components/clients-details/client-details.component';

export const CLIENT_ROUTES: Routes = [
  {
    path: '',
    component: ClientListComponent,
    title: 'Clientes'
  },
  {
    path: 'new',
    component: ClientFormComponent,
    title: 'Novo Cliente'
  },
  {
    path: ':id',
    component: ClientDetailsComponent,
    title: 'Detalhes do Cliente'
  },
  {
    path: ':id/edit',
    component: ClientFormComponent,
    title: 'Editar Cliente'
  }
];