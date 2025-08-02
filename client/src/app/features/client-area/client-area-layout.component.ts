import { Component, ChangeDetectionStrategy, signal, inject } from '@angular/core';
import { RouterModule, RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../core/services/auth.service';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-client-area-layout',
  standalone: true,
  imports: [CommonModule, RouterOutlet,RouterModule, MatIconModule],
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

        <div class="sidebar-footer">
          <button class="logout-btn" (click)="logout()" title="Sair">
            <mat-icon>logout</mat-icon>
            <span>Sair</span>
          </button>
        </div>
      </nav>
      <main class="client-area-content">
      <header class="client-area-header">
          <div class="header-content">
            <h1>Área do Cliente</h1>
            <button class="header-logout-btn" (click)="logout()" title="Sair">
              <mat-icon>logout</mat-icon>
              <span>Sair</span>
            </button>
          </div>
        </header>
        <router-outlet></router-outlet>
      </main>
    </div>
  `,
  styleUrls: ['./client-area.styles.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ClientAreaLayoutComponent {
  private authService = inject(AuthService);
  readonly mobileMenuOpen = signal(false);

  toggleMobileMenu() {
    this.mobileMenuOpen.update(open => !open);
  }

  closeMobileMenu() {
    this.mobileMenuOpen.set(false);
  }
  logout(): void {
    this.authService.logout();
  }
}
