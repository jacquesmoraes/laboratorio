import {
  ChangeDetectionStrategy,
  Component,
  inject,
  signal
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { ClientAreaService } from '../services/client-area.service';
import { ClientAreaServiceOrderDetails, orderStatusLabels } from '../models/client-area.model';

@Component({
  selector: 'app-client-area-order-details',
  standalone: true,
  imports: [CommonModule],
  template: `
    <section class="client-area-section">
      <h2>Detalhes da Ordem de Serviço</h2>

      @if (details(); as d) {
        <div class="details-header">
          <p><strong>Número:</strong> {{ d.orderNumber }}</p>
          <p><strong>Paciente:</strong> {{ d.patientName }}</p>
          <p><strong>Status:</strong> {{ orderStatusLabels[d.status] }}</p>
          <p><strong>Total:</strong> {{ d.orderTotal | currency:'BRL' }}</p>
        </div>

        <h3>Trabalhos</h3>
        <table class="client-area-table">
          <thead>
            <tr>
              <th>Tipo</th>
              <th>Quantidade</th>
              <th>Preço Unitário</th>
              <th>Cor</th>
              <th>Escala</th>
              <th>Notas</th>
            </tr>
          </thead>
          <tbody>
            @for (work of d.works; track work.workTypeId) {
              <tr>
                <td>{{ work.workTypeName }}</td>
                <td>{{ work.quantity }}</td>
                <td>{{ work.priceUnit | currency:'BRL' }}</td>
                <td>{{ work.shadeColor }}</td>
                <td>{{ work.scaleName }}</td>
                <td>{{ work.notes || '-' }}</td>
              </tr>
            }
          </tbody>
        </table>

        <h3>Etapas</h3>
        <table class="client-area-table">
          <thead>
            <tr>
              <th>Setor</th>
              <th>Data Entrada</th>
              <th>Data Saída</th>
            </tr>
          </thead>
          <tbody>
            @for (stage of d.stages; track stage.sectorId) {
              <tr>
                <td>{{ stage.sectorName }}</td>
                <td>{{ stage.dateIn | date:'dd/MM/yyyy' }}</td>
                <td>{{ stage.dateOut ? (stage.dateOut | date:'dd/MM/yyyy') : '-' }}</td>
              </tr>
            }
          </tbody>
        </table>
      } @else {
        <p>Carregando detalhes...</p>
      }
    </section>
  `,
  styleUrls: ['../client-area.styles.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ClientAreaOrderDetailsComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly service = inject(ClientAreaService);

  readonly details = signal<ClientAreaServiceOrderDetails | null>(null);
  readonly orderStatusLabels = orderStatusLabels;

  constructor() {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.loadDetails(id);
  }

  private loadDetails(id: number) {
  
  this.service.getOrderDetails(id).subscribe({
    next: d => {
      
      this.details.set(d);
    },
    error: err => {
      console.error('Error loading order details:', err);
      // Adicione um tratamento de erro mais visível
      this.details.set(null);
    }
  });
}
}
