import { Routes } from '@angular/router';
import { SectorListComponent } from './sector-list/sector-list.component';
import { SectorModalComponent } from './sector-modal/sector-modal.component';


export const SectorRoutes: Routes = [
  { path: '', component: SectorListComponent },
  { path: 'new', component: SectorModalComponent },
  { path: ':id/edit', component: SectorModalComponent },
];