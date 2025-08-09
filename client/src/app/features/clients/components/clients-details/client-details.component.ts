import { Component, inject, signal, OnInit, OnDestroy, ChangeDetectionStrategy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule, DatePipe, CurrencyPipe } from '@angular/common';
import { ClientService } from '../../services/clients.services';
import { ClientDetails, BillingMode, BillingModeLabels } from '../../models/client.interface';
import { MatIconModule } from '@angular/material/icon';
import { ClientServiceOrdersComponent } from '../client-service-orders/client-service-orders.component';
import { ClientPaymentsComponent } from '../client-payments/client-payments.component';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-client-details',
  standalone: true,
  imports: [
    CommonModule,
    MatIconModule,
    DatePipe,
    CurrencyPipe,
    ClientServiceOrdersComponent,
    ClientPaymentsComponent
  ],
  templateUrl: './client-details.component.html',
  styleUrls: ['./client-details.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ClientDetailsComponent implements OnInit, OnDestroy {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private clientService = inject(ClientService);
  private destroy$ = new Subject<void>();

  client = signal<ClientDetails | null>(null);
  loading = signal(true);

  ngOnInit(): void {
    this.route.paramMap
      .pipe(takeUntil(this.destroy$))
      .subscribe(params => {
        const id = Number(params.get('id'));
        if (id) this.loadClient(id);
      });
  }

  private loadClient(id: number): void {
    this.loading.set(true);
    this.clientService.getClientById(id).subscribe({
      next: client => {
        this.client.set(client);
        this.loading.set(false);
      },
      error: () => {
        this.client.set(null);
        this.loading.set(false);
      }
    });
  }

  hasAddress(): boolean {
    const address = this.client()?.address;
    return !!(address?.street || address?.city || address?.neighborhood);
  }

  getBillingModeLabel(mode?: BillingMode): string {
    if (mode === undefined) return 'Não informado';
    return BillingModeLabels[mode] || 'Desconhecido';
  }

  editClient(): void {
    const client = this.client();
    if (client) {
      this.router.navigate(['/clients', client.clientId, 'edit']);
    }
  }

  goBack(): void {
    this.router.navigate(['/clients']);
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
