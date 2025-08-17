import { Routes } from '@angular/router';
import { SectorListComponent } from './sector-list/sector-list.component';
import { SectorModalComponent } from './sector-modal/sector-modal.component';


export const SectorRoutes: Routes = [
  { path: '', component: SectorListComponent, title: 'Setores' },
  { path: 'new', component: SectorModalComponent, title: 'Novo Setor' },
  { path: ':id/edit', component: SectorModalComponent, title: 'Editar Setor' },
];