import { Routes } from '@angular/router';
import { ClientFormComponent } from './components/clients-form/client-form.component';
import { ClientListComponent } from './components/clients-list/client-list.component';
import { ClientDetailsComponent } from './components/clients-details/client-details.component';

export const CLIENT_ROUTES: Routes = [
  {
    path: '',
    component: ClientListComponent
  },
  {
    path: 'new',
    component: ClientFormComponent
  },
  {
    path: ':id',
    component: ClientDetailsComponent
  },
  {
    path: ':id/edit',
    component: ClientFormComponent
  }
];