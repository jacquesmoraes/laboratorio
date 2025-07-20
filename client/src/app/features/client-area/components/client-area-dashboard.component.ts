import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ClientDashboard } from '../../../core/models/client-area.model';
import { ClientAreaService } from '../services/client-area.services';


@Component({
  selector: 'app-client-area-dashboard',
  standalone: true,
  imports: [CommonModule],
  template: `
    <section class="dashboard">
      <header>
        <h2>{{ dashboard()?.clientName }}</h2>
        <p>{{ fullAddress() }}</p>
        <p>{{ dashboard()?.phoneNumber }}</p>
      </header>

      <div class="totals">
        <div class="card invoiced">
          <h3>Total Faturado</h3>
          <p>{{ dashboard()?.totalInvoiced | currency:'BRL':'symbol' }}</p>
        </div>

        <div class="card paid">
          <h3>Total Pago</h3>
          <p>{{ dashboard()?.totalPaid | currency:'BRL':'symbol' }}</p>
        </div>

        <div class="card balance">
          <h3>Saldo</h3>
          <p>{{ dashboard()?.balance | currency:'BRL':'symbol' }}</p>
        </div>
      </div>
    </section>
  `,
  styles: [`
    .dashboard {
      padding: 1rem;
      background-color: #f4f1ee;
      color: #334a52;
      border-radius: .5rem;
    }

    header {
      margin-bottom: 1rem;
    }

    header h2 {
      margin: 0;
      font-size: 1.5rem;
      color: #276678;
    }

    header p {
      margin: 0;
      font-size: .875rem;
    }

    .totals {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
      gap: 1rem;
    }

    .card {
      padding: 1rem;
      border-radius: .5rem;
      text-align: center;
      box-shadow: 0 1px 3px rgba(0,0,0,0.1);
    }

    .card.invoiced {
      background-color: #96afb8;
    }

    .card.paid {
      background-color: #a288a9;
    }

    .card.balance {
      background-color: #276678;
      color: #f4f1ee;
    }
  `],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ClientAreaDashboardComponent {
  private readonly service = inject(ClientAreaService);

  readonly dashboard = signal<ClientDashboard | null>(null);

  constructor() {
    this.loadDashboard();
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
