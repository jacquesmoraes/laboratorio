import { Routes } from '@angular/router';
import { authGuard, adminGuard, clientGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  // Homepage pública (site institucional)
  {
    path: '',
    loadComponent: () => import('./pages/homepage.component').then(m => m.HomepageComponent)
  },
  
  // Rotas públicas de autenticação
  {
    path: 'login',
    loadChildren: () => import('./core/auth/auth.routes').then(m => m.AUTH_ROUTES)
  },
  {
    path: 'register',
    loadChildren: () => import('./core/auth/auth.routes').then(m => m.AUTH_ROUTES)
  },
  {
    path: 'complete-first-access',
    loadChildren: () => import('./core/auth/auth.routes').then(m => m.AUTH_ROUTES)
  },
  {
    path: 'forgot-password',
    loadChildren: () => import('./core/auth/auth.routes').then(m => m.AUTH_ROUTES)
  },
  {
    path: 'reset-password',
    loadChildren: () => import('./core/auth/auth.routes').then(m => m.AUTH_ROUTES)
  },
  
  // Área administrativa
  {
    path: 'admin',
    canActivate: [authGuard, adminGuard],
    loadComponent: () => import('./shared/components/layout/layout.component').then(m => m.LayoutComponent),
    children: [
      { path: '', loadChildren: () => import('./shared/components/admin-dashboard/admin-dashboard.routes').then(m => m.HOME_ROUTES) },
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
      { path: 'user-management', loadChildren: () => import('./features/user-management/user-management.routes').then(m => m.USER_MANAGEMENT_ROUTES) },
    ]
  },
  
  // Área do cliente
  {
    path: 'client-area',
    canActivate: [authGuard, clientGuard],
    loadChildren: () => import('./features/client-area/client-area.routes').then(m => m.CLIENT_AREA_ROUTES)
  },
  
  // Rota 404
  { path: '**', redirectTo: '' }
];