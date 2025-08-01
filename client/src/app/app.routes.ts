import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./shared/components/layout/layout.component').then(m => m.LayoutComponent),
    children: [
      { path: '', loadChildren: () => import('./shared/components/home/home.routes').then(m => m.HOME_ROUTES) },
      { path: 'sectors', loadChildren: () => import('./features/sectors/sector.routes').then(m => m.SectorRoutes) },
      { path: 'work-sections', loadChildren: () => import('./features/works/work-section.routes').then(m => m.WORK_SECTION_ROUTES) },
      { path: 'work-types', loadChildren: () => import('./features/works/work-types.routes').then(m => m.WORK_TYPE_ROUTES) },
      { path: 'scales', loadChildren: () => import('./features/production/scale.routes').then(m => m.SCALE_ROUTES) },
      { path: 'shades', loadChildren: () => import('./features/production/shade.routes').then(m => m.SHADE_ROUTES) },
      { path: 'settings', loadChildren: () => import('./features/settings/settings.routes').then(m => m.SETTINGS_ROUTES) },
      { path: 'clients', loadChildren: () => import('./features/clients/client.routes').then(m => m.CLIENT_ROUTES) },
      { path: 'table-price', loadChildren: () => import('./features/table-price/table-price.routes').then(m => m.TABLE_PRICE_ROUTES) },
      { path: 'service-orders', loadChildren: () => import('./features/service-order/service-order.routes').then(m => m.SERVICE_ORDERS_ROUTES) },
      { path: 'billing', loadChildren: () => import('./features/billing/billing.routes').then(m => m.BILLING_ROUTES) },
       { path: 'payments', loadChildren: () => import('./features/payments/payment.routes').then(m => m.PAYMENT_ROUTES) },


    ]
  },
   {
    path: 'client-area',
    loadChildren: () => import('./features/client-area/client-area.routes').then(m => m.CLIENT_AREA_ROUTES)
  },
  {
    path: '**',
    redirectTo: '/'
  }
];