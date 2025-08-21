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

      <!-- Próximas Entregas (apenas TryIn e FinalDelivery) -->
      @if (dashboard()?.upcomingDeliveries && dashboard()!.upcomingDeliveries.length > 0) {
        <div class="upcoming-deliveries">
          <h3>Próximas Entregas</h3>
          <div class="deliveries-grid">
            @for (delivery of dashboard()!.upcomingDeliveries; track delivery.scheduleId) {
              <div class="delivery-card" [class.overdue]="delivery.isOverdue">
                <div class="delivery-header">
                  <h4>OS {{ delivery.orderNumber }}</h4>
                  @if (delivery.isOverdue) {
                    <span class="status overdue">Atrasado</span>
                  } @else if (isToday(delivery.scheduledDate)) {
                    <span class="status today">Hoje</span>
                  } @else {
                    <span class="status scheduled">Agendado</span>
                  }
                </div>
                <div class="delivery-details">
                  <p><strong>Paciente:</strong> {{ delivery.patientName }}</p>
                  <p><strong>Data:</strong> {{ delivery.scheduledDate | date:'dd/MM/yyyy' }}</p>
                  <p><strong>Tipo:</strong> {{ getDeliveryTypeLabel(delivery.deliveryType) }}</p>
                </div>
              </div>
            }
          </div>

          
        </div>
      }@else {
    <!-- Empty State -->
    <div class="empty-state">
      <div class="empty-state-icon">
        <svg width="64" height="64" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5">
          <rect x="3" y="4" width="18" height="18" rx="2" ry="2"/>
          <line x1="16" y1="2" x2="16" y2="6"/>
          <line x1="8" y1="2" x2="8" y2="6"/>
          <line x1="3" y1="10" x2="21" y2="10"/>
        </svg>
      </div>
      <h4>Nenhuma entrega agendada</h4>
      <p>Suas próximas entregas aparecerão aqui quando forem agendadas.</p>
    </div>

      }
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
  isToday(dateStr: string): boolean {
    const date = new Date(dateStr);
    const today = new Date();
    return date.toDateString() === today.toDateString();
  }

  getDeliveryTypeLabel(type: string): string {
    return type === 'TryIn' ? 'Prova' : 'Entrega Final';
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
