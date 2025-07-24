import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ClientAreaService } from '../services/client-area.services';
import { ClientDashboard } from '../models/client-area.model';


@Component({
  selector: 'app-client-area-dashboard',
  standalone: true,
  imports: [CommonModule],
  template: `
    <section class="client-area-section client-area-dashboard">
      <div class="client-info">
        <h2>{{ dashboard()?.clientName }}</h2>
        <p>{{ fullAddress() }}</p>
        <p>{{ dashboard()?.phoneNumber }}</p>
      </div>

      <div class="totals-grid">
        <div class="total-card invoiced">
          <h3>Total Faturado</h3>
          <p class="amount neutral">{{ dashboard()?.totalInvoiced | currency:'BRL':'symbol' }}</p>
        </div>

        <div class="total-card paid">
          <h3>Total Pago</h3>
          <p class="amount positive">{{ dashboard()?.totalPaid | currency:'BRL':'symbol' }}</p>
        </div>

        <div class="total-card balance">
          <h3>Saldo</h3>
          <p class="amount" [class]="getBalanceClass()">{{ dashboard()?.balance | currency:'BRL':'symbol' }}</p>
        </div>
      </div>
    </section>
  `,
  styleUrls: ['../client-area.styles.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ClientAreaDashboardComponent {
  private readonly service = inject(ClientAreaService);

  readonly dashboard = signal<ClientDashboard | null>(null);

  constructor() {
    this.loadDashboard();
  }
  getBalanceClass(): string {
    const balance = this.dashboard()?.balance || 0;
    return balance > 0 ? 'positive' : balance < 0 ? 'negative' : 'neutral';
  }
  private loadDashboard() {
    this.service.getDashboard().subscribe(d => this.dashboard.set(d));
  }

  fullAddress(): string {
    const d = this.dashboard();
    if (!d) return '';
    return `${d.street}, ${d.number} ${d.complement} - ${d.neighborhood}, ${d.city}`;
  }
}
