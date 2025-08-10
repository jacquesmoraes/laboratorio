import { Component, inject, signal, OnInit, ChangeDetectionStrategy, DestroyRef, computed } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule, DatePipe, CurrencyPipe } from '@angular/common';
import { ClientService } from '../../services/clients.services';
import { ClientDetails, BillingMode, BillingModeLabels } from '../../models/client.interface';
import { MatIconModule } from '@angular/material/icon';
import { ClientServiceOrdersComponent } from '../client-service-orders/client-service-orders.component';
import { ClientPaymentsComponent } from '../client-payments/client-payments.component';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

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
export class ClientDetailsComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private clientService = inject(ClientService);
  private readonly destroyRef = inject(DestroyRef);

  client = signal<ClientDetails | null>(null);
  loading = signal(true);

  ngOnInit(): void {
    this.route.paramMap 
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(params => {
        const id = Number(params.get('id'));
        if (id) this.loadClient(id);
      });
  }

  private loadClient(id: number): void {
    this.loading.set(true);
    this.clientService.getClientById(id)
    .pipe(takeUntilDestroyed(this.destroyRef))
    .subscribe({
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

  protected readonly hasAddress = computed(() => {
    const address = this.client()?.address;
    return !!(address?.street || address?.city || address?.neighborhood);
  }); 

  protected readonly getBillingModeLabel = computed(() => {
    return (mode?: BillingMode): string => {
      if (mode === undefined) return 'Não informado';
      return BillingModeLabels[mode] || 'Desconhecido';
    };
  });

  editClient(): void {
    const client = this.client();
    if (client) {
      this.router.navigate(['/clients', client.clientId, 'edit']);
    }
  }

  goBack(): void {
    this.router.navigate(['/clients']);
  }

}
