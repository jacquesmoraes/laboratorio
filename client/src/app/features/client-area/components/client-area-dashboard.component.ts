import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ClientAreaService } from '../services/client-area.service';
import { ClientDashboard, MonthlyBalanceRecord   } from '../models/client-area.model';

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

      <!-- Saldo Atual -->
      <div class="current-balance">
        <h3>Saldo Atual</h3>
        <div class="balance-card">
          <p class="amount" [class]="getCurrentBalanceClass()">
            {{ getCurrentBalance() | currency:'BRL':'symbol' }}
          </p>
          <span class="balance-label" [class]="getCurrentBalanceClass()">
            {{ getCurrentBalanceLabel() }}
          </span>
        </div>
      </div>

      <!-- Histórico Mensal -->
      @if (dashboard()?.monthlyBalances && dashboard()!.monthlyBalances.length > 0) {
        <div class="monthly-balances">
          <h3>Histórico dos Últimos 12 Meses</h3>
          
          <div class="months-tabs">
            @for (month of dashboard()!.monthlyBalances; track month.monthName) {
              <button 
                class="month-tab" 
                [class.active]="selectedMonth() === month"
                [class.current-month]="month.isCurrentMonth"
                (click)="selectMonth(month)">
                {{ month.monthName }}
              </button>
            }
          </div>

          @if (selectedMonth()) {
            <div class="month-details">
              <div class="month-summary">
                <div class="summary-card">
                  <h4>Faturado</h4>
                  <p class="amount neutral">{{ selectedMonth()!.invoiced | currency:'BRL':'symbol' }}</p>
                </div>
                <div class="summary-card">
                  <h4>Pago</h4>
                  <p class="amount positive">{{ selectedMonth()!.paid | currency:'BRL':'symbol' }}</p>
                </div>
                <div class="summary-card">
                  <h4>Saldo do Mês</h4>
                  <p class="amount" [class]="getMonthBalanceClass(selectedMonth()!)">
                    {{ selectedMonth()!.balance | currency:'BRL':'symbol' }}
                  </p>
                </div>
              </div>
            </div>
          }
        </div>
      }

      <!-- Próximas Entregas -->
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
      } @else {
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
  readonly selectedMonth = signal<MonthlyBalanceRecord | null>(null);

  constructor() {
    this.loadDashboard();
  }

  getCurrentBalance(): number {
    const currentMonth = this.dashboard()?.monthlyBalances.find(m => m.isCurrentMonth);
    return currentMonth?.balance || 0;
  }

  getCurrentBalanceClass(): string {
    const balance = this.getCurrentBalance();
    return balance > 0 ? 'positive' : balance < 0 ? 'negative' : 'neutral';
  }

  getCurrentBalanceLabel(): string {
    const balance = this.getCurrentBalance();
    if (balance > 0) return 'Crédito Disponível';
    if (balance < 0) return 'Valor Pendente';
    return 'Conta em Dia';
  }

  getMonthBalanceClass(month: MonthlyBalanceRecord): string {
    return month.balance > 0 ? 'positive' : month.balance < 0 ? 'negative' : 'neutral';
  }

  selectMonth(month: MonthlyBalanceRecord): void {
    this.selectedMonth.set(month);
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
    this.service.getDashboard().subscribe(dashboard => {
      this.dashboard.set(dashboard);
      // Selecionar automaticamente o mês atual
      const currentMonth = dashboard?.monthlyBalances.find(m => m.isCurrentMonth);
      this.selectedMonth.set(currentMonth || null);
    });
  }

  fullAddress(): string {
    const d = this.dashboard();
    if (!d) return '';
    return `${d.street}, ${d.number} ${d.complement} - ${d.neighborhood}, ${d.city}`;
  }
}
