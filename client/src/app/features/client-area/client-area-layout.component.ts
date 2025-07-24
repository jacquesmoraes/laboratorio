import { Component, ChangeDetectionStrategy, signal } from '@angular/core';
import { RouterModule, RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-client-area-layout',
  standalone: true,
  imports: [CommonModule, RouterOutlet,RouterModule],
  template: `
    <div class="client-area-layout">
      <!-- Menu Mobile Toggle -->
      <button class="mobile-menu-toggle" (click)="toggleMobileMenu()" [class.active]="mobileMenuOpen()">
        <span></span>
        <span></span>
        <span></span>
      </button>

      

      <nav class="client-area-sidebar" [class.mobile-open]="mobileMenuOpen()">
        <div class="sidebar-header">
          <h2>Área do Cliente</h2>
          <button class="close-mobile-menu" (click)="closeMobileMenu()">×</button>
        </div>
        <ul>
          <li><a routerLink="/client-area" routerLinkActive="active" [routerLinkActiveOptions]="{ exact: true }" (click)="closeMobileMenu()">Dashboard</a></li>
          <li><a routerLink="/client-area/payments" routerLinkActive="active" (click)="closeMobileMenu()">Pagamentos</a></li>
          <li><a routerLink="/client-area/invoices" routerLinkActive="active" (click)="closeMobileMenu()">Faturas</a></li>
          <li><a routerLink="/client-area/orders" routerLinkActive="active" (click)="closeMobileMenu()">Ordens de Serviço</a></li>
        </ul>
      </nav>
      <main class="client-area-content">
        <router-outlet></router-outlet>
      </main>
    </div>
  `,
  styleUrls: ['./client-area.styles.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ClientAreaLayoutComponent {
  readonly mobileMenuOpen = signal(false);

  toggleMobileMenu() {
    this.mobileMenuOpen.update(open => !open);
  }

  closeMobileMenu() {
    this.mobileMenuOpen.set(false);
  }
}
