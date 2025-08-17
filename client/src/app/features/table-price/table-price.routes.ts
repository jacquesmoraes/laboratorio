import { Routes } from '@angular/router';

export const TABLE_PRICE_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./components/table-price-list/table-price-list.component')
      .then(m => m.TablePriceListComponent),
    title: 'Tabelas de Preços'
    
  },
  {
    path: 'new',
    loadComponent: () => import('./components/table-price-form/table-price-form.component')
      .then(m => m.TablePriceFormComponent),
    title: 'Nova Tabela de Preços'
    
  },
  {
    path: ':id',
    loadComponent: () => import('./components/table-price-details/table-price-details.component')
      .then(m => m.TablePriceDetailsComponent),
    title: 'Detalhes da Tabela de Preços'
    
  },
  {
    path: ':id/edit',
    loadComponent: () => import('./components/table-price-form/table-price-form.component')
      .then(m => m.TablePriceFormComponent),
    title: 'Editar Tabela de Preços'
    
  }
];