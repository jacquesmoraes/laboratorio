import { Component, ChangeDetectionStrategy, inject } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { LoadingSpinnerComponent } from './shared/components/loading-spinner/loading-spinner.component/loading-spinner.component';


@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    RouterOutlet,
    LoadingSpinnerComponent,
    
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
     <app-loading-spinner />
     <router-outlet></router-outlet>
  `
})
export class App {
  protected title = 'Elite Laboratorio';
  private readonly router = inject(Router);

  isClientArea(): boolean {
    return this.router.url.startsWith('/client-area');
  }
}