import { Routes } from '@angular/router';


export const routes: Routes = [
  { path: 'sectors', loadChildren: () => import('./features/sectors/sector.routes').then(m => m.SectorRoutes)},
  {path: 'work-sections', loadChildren: () => import('./features/works/work-section.routes').then(m => m.WORK_SECTION_ROUTES)},
  { path: 'work-types',   loadChildren: () => import('./features/works/work-types.routes').then(m => m.WORK_TYPE_ROUTES)},
  {
    path: '**',
    redirectTo: '/sectors'
  }
];