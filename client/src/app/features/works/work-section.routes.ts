import { Routes } from '@angular/router';
import { WorkSectionFormComponent } from './components/work-section-form-component/work-section-form-component';
import { WorkSectionListComponent } from './components/work-section-list-component/work-section-list.component';

export const WORK_SECTION_ROUTES: Routes = [
  {
    path: '',
    component: WorkSectionListComponent
  },
  {
    path: 'new',
    component: WorkSectionFormComponent
  },
  {
    path: 'edit/:id',
    component: WorkSectionFormComponent
  }
];