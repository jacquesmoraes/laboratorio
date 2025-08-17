import { Routes } from '@angular/router';

import { UserDetailsComponent } from './components/user-details-component/user-details.component';
import { UserFormComponent } from './components/user-form-component/user-form.component';
import { UserListComponent } from './components/user-list-component/user-list.component';
import { adminGuard } from '../../core/guards/auth.guard';

export const USER_MANAGEMENT_ROUTES: Routes = [
  { path: '', component: UserListComponent,title:'gerenciar usuarios', canActivate: [adminGuard] },
  { path: 'new', component: UserFormComponent,title:'novo usuario', canActivate: [adminGuard] },
  { path: ':userId', component: UserDetailsComponent,title:'detalhes do usuario', canActivate: [adminGuard] },
  { path: ':userId/edit', component: UserFormComponent,title:'editar usuario', canActivate: [adminGuard] }
];

