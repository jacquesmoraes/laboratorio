import { Routes } from '@angular/router';

import { UserDetailsComponent } from './components/user-details-component/user-details.component';
import { UserFormComponent } from './components/user-form-component/user-form.component';
import { UserListComponent } from './components/user-list-component/user-list.component';
import { adminGuard } from '../../core/guards/auth.guard';

export const USER_MANAGEMENT_ROUTES: Routes = [
  { path: '', component: UserListComponent, canActivate: [adminGuard] },
  { path: 'new', component: UserFormComponent, canActivate: [adminGuard] },
  { path: ':userId', component: UserDetailsComponent, canActivate: [adminGuard] },
  { path: ':userId/edit', component: UserFormComponent, canActivate: [adminGuard] }
];

