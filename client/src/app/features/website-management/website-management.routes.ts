import { Routes } from '@angular/router';

export const WEBSITE_MANAGEMENT_ROUTES: Routes = [
  {
    path: '',
    redirectTo: 'cases',
    pathMatch: 'full'
  },
  {
    path: 'cases',
    loadComponent: () => import('./components/website-cases/website-case-list-component/website-case-list.component')
      .then(m => m.WebsiteCaseListComponent)
  },
  {
    path: 'cases/new',
    loadComponent: () => import('./components/website-cases/website-case-form-component/website-case-form.component')
      .then(m => m.WebsiteCaseFormComponent)
  },
  {
    path: 'cases/:id',
    loadComponent: () => import('./components/website-cases/website-case-details-component/website-case-details.component')
      .then(m => m.WebsiteCaseDetailsComponent)
  },
  {
    path: 'cases/:id/edit',
    loadComponent: () => import('./components/website-cases/website-case-form-component/website-case-form.component')
      .then(m => m.WebsiteCaseFormComponent)
  },
  {
    path: 'work-types',
    loadComponent: () => import('./components/website-worktypes/website-worktype-list-component/website-worktype-list.component')
      .then(m => m.WebsiteWorkTypeListComponent)
  },
  {
    path: 'work-types/new',
    loadComponent: () => import('./components/website-worktypes/website-worktype-form-component/website-worktype-form.component')
      .then(m => m.WebsiteWorkTypeFormComponent)
  },
  {
    path: 'work-types/:id/edit',
    loadComponent: () => import('./components/website-worktypes/website-worktype-form-component/website-worktype-form.component')
      .then(m => m.WebsiteWorkTypeFormComponent)
  }
];