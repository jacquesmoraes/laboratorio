import { Component, ChangeDetectionStrategy } from '@angular/core';
import { RouterModule, RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-client-area-layout',
  standalone: true,
  imports: [CommonModule, RouterOutlet,RouterModule],
  template: `
    <div class="client-area-layout">
      <nav class="client-area-sidebar">
        <h2>Área do Cliente</h2>
        <ul>
          <li><a routerLink="/client-area" routerLinkActive="active" [routerLinkActiveOptions]="{ exact: true }">Dashboard</a></li>
          <li><a routerLink="/client-area/payments" routerLinkActive="active">Pagamentos</a></li>
          <li><a routerLink="/client-area/invoices" routerLinkActive="active">Faturas</a></li>
          <li><a routerLink="/client-area/orders" routerLinkActive="active">Ordens de Serviço</a></li>
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
export class ClientAreaLayoutComponent {}
