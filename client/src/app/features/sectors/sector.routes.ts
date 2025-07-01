import { Routes } from '@angular/router';
import { SectorListComponent } from './sector-list/sector-list.component';
import { SectorFormComponent } from './sector-form/sector-form.component';

export const SectorRoutes: Routes = [
  { path: '', component: SectorListComponent },
  { path: 'new', component: SectorFormComponent },
  { path: ':id/edit', component: SectorFormComponent },
];