// client/src/app/features/client-area/components/dashborad/client-area-dashboard.component.ts
import { Component, OnInit, signal, inject, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

import { ClientAreaInvoicesComponent } from '../invoices/client-area-invoices.component';
import { ClientAreaOrdersComponent } from '../orders/client-area-orders.component';
import { ClientAreaPaymentsComponent } from '../payments/client-area-payments.component';
import { ClientAreaService } from '../../services/client-area.service';
import { ClientDashboardData } from '../../models/client-area.interface';

@Component({
  selector: 'app-client-area-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    ClientAreaInvoicesComponent,
    ClientAreaOrdersComponent,
    ClientAreaPaymentsComponent
  ],
  template: `
    <header>
      <h1>Área do Cliente</h1>
    </header>

    <div class="container">
      <!-- Resumo -->
      <div class="header-summary">
        <h2>{{ dashboardData()?.clientName }}</h2>
        <p>{{ dashboardData()?.street }}, {{ dashboardData()?.number }} — {{ dashboardData()?.city }}</p>
        <p>Telefone: {{ dashboardData()?.phoneNumber }}</p>
        <div class="summary-cards">
          <div class="summary-card">
            <h4>Total Faturado</h4>
            <div>{{ formatCurrency(dashboardData()?.totalInvoiced || 0) }}</div>
          </div>
          <div class="summary-card">
            <h4>Total Pago</h4>
            <div>{{ formatCurrency(dashboardData()?.totalPaid || 0) }}</div>
          </div>
          <div class="summary-card">
            <h4>Saldo</h4>
            <div>{{ formatCurrency(dashboardData()?.balance || 0) }}</div>
          </div>
        </div>
      </div>

      <!-- Componentes reutilizados -->
      <section>
        <h3>Pagamentos Recentes</h3>
        <app-client-area-payments 
          [limit]="5" 
          [showPagination]="true" 
          [showHeader]="true"
          [showViewAllLink]="true"
          />
      </section>

      <section>
        <h3>Ordens Recentes</h3>
        <app-client-area-orders 
          [limit]="5" 
          [showPagination]="true" 
          [showHeader]="true"
          [showViewAllLink]="true" />
      </section>

      <section>
        <h3>Faturas Recentes</h3>
        <app-client-area-invoices 
          [limit]="5" 
          [showPagination]="true" 
          [showHeader]="true"
          [showViewAllLink]="true" />
      </section>
    </div>
  `,
  styleUrls: ['./client-area-dashboard.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ClientAreaDashboardComponent implements OnInit {
  private readonly clientAreaService = inject(ClientAreaService);
  dashboardData = signal<ClientDashboardData | null>(null);

  ngOnInit() {
    this.loadDashboard();
  }

  loadDashboard() {
    this.clientAreaService.getDashboard().subscribe({
      next: (data) => this.dashboardData.set(data),
      error: (err) => console.error('Erro ao carregar dashboard', err)
    });
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(value);
  }
}