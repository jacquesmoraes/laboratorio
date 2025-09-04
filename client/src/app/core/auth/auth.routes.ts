import { Routes } from '@angular/router';

export const AUTH_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./login/login.component').then(m => m.LoginComponent)
  },
  {
    path: 'register',
    loadComponent: () => import('./register/register-component').then(m => m.RegisterComponent)
  },
  
  {
    path: 'change-password',
    loadComponent: () => import('./change-password/change-password.component').then(m => m.ChangePasswordComponent)
  },
  {
    path: 'forgot-password',
    loadComponent: () => import('./forgot-password/forgot-password.component').then(m => m.ForgotPasswordComponent)
  }
];