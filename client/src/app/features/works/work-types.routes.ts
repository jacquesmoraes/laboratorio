import { Routes } from '@angular/router';
import { WorkTypeFormComponent } from './components/work-type-form-component/work-type-form-component';
import { WorkTypeListComponent } from './components/work-type-list-component/work-type-list-component';

export const WORK_TYPE_ROUTES: Routes = [
  {
    path: '',
    component: WorkTypeListComponent
  },
  {
    path: 'new',
    component: WorkTypeFormComponent
  },
  {
    path: 'edit/:id',
    component: WorkTypeFormComponent
  }
];