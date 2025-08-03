import { Component, ChangeDetectionStrategy, signal, inject } from '@angular/core';
import { Router, RouterModule, RouterOutlet } from '@angular/router';
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

        <div class="sidebar-actions">
          <button class="sidebar-btn" title="Notificações">
            <mat-icon>notification_important</mat-icon>
          </button>
          <button class="sidebar-btn" title="Alterar Senha" (click)="changePassword()">
            <mat-icon>lock_reset</mat-icon>
          </button>
          <button class="sidebar-btn logout-btn" title="Sair" (click)="logout()">
            <mat-icon>logout</mat-icon>
          </button>
        </div>
      </nav>


      <main class="client-area-content">
        <!-- Header principal -->
       
        
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
  private router = inject(Router);
  toggleMobileMenu() {
    this.mobileMenuOpen.update(open => !open);
  }

  closeMobileMenu() {
    this.mobileMenuOpen.set(false);
  }
  logout(): void {
    this.authService.logout();
  }
 changePassword(): void {
  this.router.navigate(['/client-area/change-password']);
}
}
