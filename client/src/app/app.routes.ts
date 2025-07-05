import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./shared/components/layout/layout.component').then(m => m.LayoutComponent),
    children: [
      { path: '', loadChildren: () => import('./shared/components/home/home.routes').then(m => m.HOME_ROUTES) },
      { path: 'sectors', loadChildren: () => import('./features/sectors/sector.routes').then(m => m.SectorRoutes)},
      { path: 'work-sections', loadChildren: () => import('./features/works/work-section.routes').then(m => m.WORK_SECTION_ROUTES)},
      { path: 'work-types', loadChildren: () => import('./features/works/work-types.routes').then(m => m.WORK_TYPE_ROUTES)},
      { path: 'scales', loadChildren: () => import('./features/production/scale.routes').then(m => m.SCALE_ROUTES)},
      { path: 'shades', loadChildren: () => import('./features/production/shade.routes').then(m => m.SHADE_ROUTES)},
    ]
  },
  {
    path: '**',
    redirectTo: '/'
  }
];