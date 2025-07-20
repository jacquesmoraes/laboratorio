import { Component, ChangeDetectionStrategy } from '@angular/core';
import { RouterModule, RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-client-area-layout',
  standalone: true,
  imports: [CommonModule, RouterOutlet,RouterModule],
  template: `
    <div class="layout">
      <nav class="sidebar">
        <h2>Área do Cliente</h2>
        <ul>
          <li><a routerLink="/client-area" routerLinkActive="active" [routerLinkActiveOptions]="{ exact: true }">Dashboard</a></li>
          <li><a routerLink="/client-area/payments" routerLinkActive="active">Pagamentos</a></li>
          <li><a routerLink="/client-area/invoices" routerLinkActive="active">Faturas</a></li>
          <li><a routerLink="/client-area/orders" routerLinkActive="active">Ordens de Serviço</a></li>
        </ul>
      </nav>
      <main class="content">
        <router-outlet></router-outlet>
      </main>
    </div>
  `,
  styles: [`
    .layout {
      display: flex;
      min-height: 100vh;
    }

    .sidebar {
      background-color: #276678;
      color: #f4f1ee;
      padding: 1rem;
      width: 220px;
    }

    .sidebar h2 {
      margin-top: 0;
      font-size: 1.25rem;
      margin-bottom: 1rem;
    }

    .sidebar ul {
      list-style: none;
      padding: 0;
      margin: 0;
    }

    .sidebar li {
      margin-bottom: .5rem;
    }

    .sidebar a {
      color: #f4f1ee;
      text-decoration: none;
      display: block;
      padding: .25rem 0;
    }

    .sidebar a.active {
      font-weight: bold;
      color: #a288a9;
    }

    .content {
      flex: 1;
      padding: 1rem;
      background-color: #f4f1ee;
    }
  `],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ClientAreaLayoutComponent {}
