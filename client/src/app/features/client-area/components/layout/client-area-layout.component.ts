import { Component, signal, inject, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';

@Component({
  selector: 'app-client-area-layout',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="client-area-container">
      <!-- Header -->
      <header class="header">
        <div class="header-content">
          <h1>Área do Cliente</h1>
          <nav class="nav-menu">
            <a 
              routerLink="dashboard" 
              routerLinkActive="active"
              class="nav-item">
              Dashboard
            </a>
            <a 
              routerLink="invoices" 
              routerLinkActive="active"
              class="nav-item">
              Faturas
            </a>
            <a 
              routerLink="orders" 
              routerLinkActive="active"
              class="nav-item">
              Ordens
            </a>
            <a 
              routerLink="payments" 
              routerLinkActive="active"
              class="nav-item">
              Pagamentos
            </a>
          </nav>
        </div>
      </header>

      <!-- Main Content -->
      <main class="main-content">
        <router-outlet />
      </main>
    </div>
  `,
  styles: [`
    .client-area-container {
      min-height: 100vh;
      background-color: #f4f1ee;
    }

    .header {
      background-color: #276678;
      color: white;
      padding: 1rem 0;
      box-shadow: 0 2px 4px rgba(0,0,0,0.1);
    }

    .header-content {
      max-width: 1200px;
      margin: 0 auto;
      padding: 0 1rem;
      display: flex;
      justify-content: space-between;
      align-items: center;
    }

    .header h1 {
      margin: 0;
      font-size: 1.5rem;
      font-weight: 600;
    }

    .nav-menu {
      display: flex;
      gap: 2rem;
    }

    .nav-item {
      color: white;
      text-decoration: none;
      padding: 0.5rem 1rem;
      border-radius: 4px;
      transition: background-color 0.2s;
    }

    .nav-item:hover {
      background-color: rgba(255,255,255,0.1);
    }

    .nav-item.active {
      background-color: rgba(255,255,255,0.2);
    }

    .main-content {
      max-width: 1200px;
      margin: 0 auto;
      padding: 2rem 1rem;
    }
  `],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ClientAreaLayoutComponent {
  private readonly router = inject(Router);
}