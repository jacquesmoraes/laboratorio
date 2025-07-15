import { Component, signal, inject, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

import { ClientAreaService } from '../../services/client-area.service';
import { ClientDashboardData } from '../../models/client-area.interface';

@Component({
  selector: 'app-client-area-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="dashboard">
      <!-- Informações do Cliente -->
  <section class="client-info">
  <div class="client-card">
    <h2>{{ dashboardData()?.clientName }}</h2>
    <div class="client-details">
      <p>
        <strong>Endereço:</strong> 
        {{ dashboardData()?.street }}, {{ dashboardData()?.number }}
        @if (dashboardData()?.complement) {
          <span> - {{ dashboardData()?.complement }}</span>
        }
      </p>
      <p>
        <strong>Cidade:</strong> {{ dashboardData()?.city }}
      </p>
      @if (dashboardData()?.phoneNumber) {
        <p>
          <strong>Telefone:</strong> {{ dashboardData()?.phoneNumber }}
        </p>
      }
    </div>
  </div>
</section>


      <!-- Cards de Resumo -->
      <section class="summary-cards">
        <div class="summary-card">
          <div class="card-icon">💰</div>
          <div class="card-content">
            <h3>Total Faturado</h3>
            <div class="card-value">{{ formatCurrency(dashboardData()?.totalInvoiced || 0) }}</div>
          </div>
        </div>

        <div class="summary-card">
          <div class="card-icon">💳</div>
          <div class="card-content">
            <h3>Total Pago</h3>
            <div class="card-value">{{ formatCurrency(dashboardData()?.totalPaid || 0) }}</div>
          </div>
        </div>

        <div class="summary-card">
          <div class="card-icon">⚖️</div>
          <div class="card-content">
            <h3>Saldo</h3>
            <div class="card-value" [class.negative]="(dashboardData()?.balance || 0) < 0">
              {{ formatCurrency(dashboardData()?.balance || 0) }}
            </div>
          </div>
        </div>
      </section>

      <!-- Links Rápidos -->
      <section class="quick-links">
        <h3>Acesso Rápido</h3>
        <div class="links-grid">
          <a routerLink="../invoices" class="quick-link">
            <div class="link-icon">📄</div>
            <span>Ver Faturas</span>
          </a>
          <a routerLink="../orders" class="quick-link">
            <div class="link-icon">📋</div>
            <span>Ver Ordens</span>
          </a>
          <a routerLink="../payments" class="quick-link">
            <div class="link-icon">💳</div>
            <span>Ver Pagamentos</span>
          </a>
        </div>
      </section>
    </div>
  `,
  styles: [`
    .dashboard {
      display: flex;
      flex-direction: column;
      gap: 2rem;
    }

    .client-info {
      background: white;
      border-radius: 8px;
      padding: 1.5rem;
      box-shadow: 0 2px 4px rgba(0,0,0,0.1);
    }

    .client-card h2 {
      margin: 0 0 1rem 0;
      color: #276678;
      font-size: 1.5rem;
    }

    .client-details p {
      margin: 0.5rem 0;
      color: #334a52;
    }

    .summary-cards {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
      gap: 1rem;
    }

    .summary-card {
      background: white;
      border-radius: 8px;
      padding: 1.5rem;
      box-shadow: 0 2px 4px rgba(0,0,0,0.1);
      display: flex;
      align-items: center;
      gap: 1rem;
    }

    .card-icon {
      font-size: 2rem;
    }

    .card-content h3 {
      margin: 0 0 0.5rem 0;
      color: #334a52;
      font-size: 0.9rem;
      font-weight: 500;
    }

    .card-value {
      font-size: 1.5rem;
      font-weight: 600;
      color: #276678;
    }

    .card-value.negative {
      color: #dc3545;
    }

    .quick-links {
      background: white;
      border-radius: 8px;
      padding: 1.5rem;
      box-shadow: 0 2px 4px rgba(0,0,0,0.1);
    }

    .quick-links h3 {
      margin: 0 0 1rem 0;
      color: #276678;
    }

    .links-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
      gap: 1rem;
    }

    .quick-link {
      display: flex;
      flex-direction: column;
      align-items: center;
      padding: 1.5rem;
      border: 2px solid #96afb8;
      border-radius: 8px;
      text-decoration: none;
      color: #334a52;
      transition: all 0.2s;
    }

    .quick-link:hover {
      border-color: #276678;
      background-color: #f8f9fa;
    }

    .link-icon {
      font-size: 2rem;
      margin-bottom: 0.5rem;
    }

    @media (max-width: 768px) {
      .summary-cards {
        grid-template-columns: 1fr;
      }
      
      .links-grid {
        grid-template-columns: 1fr;
      }
    }
  `],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ClientAreaDashboardComponent {
  private readonly clientAreaService = inject(ClientAreaService);
  dashboardData = signal<ClientDashboardData | null>(null);

  constructor() {
    this.loadDashboardData();
  }

  private loadDashboardData() {
    this.clientAreaService.getDashboard().subscribe({
      next: (data) => {
        this.dashboardData.set(data);
      },
      error: (err) => console.error('Erro ao carregar dados do dashboard', err)
    });
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('pt-BR', { 
      style: 'currency', 
      currency: 'BRL' 
    }).format(value);
  }
}