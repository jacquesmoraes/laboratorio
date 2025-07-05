import { Component, inject, signal, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ClientService } from '../../services/clients.services';
import { ClientDetails, BillingModeLabels } from '../../models/client.interface';

@Component({
  selector: 'app-client-details',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './client-details.component.html',
  styleUrl:'./client-details.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ClientDetailsComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private clientService = inject(ClientService);

  client = signal<ClientDetails | null>(null);
  loading = signal(true);

  ngOnInit() {
  
  const clientId = this.route.snapshot.paramMap.get('id');
  
  if (clientId) {
    this.loadClient(parseInt(clientId));
  }
}

private loadClient(id: number) {
  
  this.loading.set(true);
  this.clientService.getClientById(id).subscribe({
    next: (client) => {
      
      this.client.set(client);
      this.loading.set(false);
    },
    error: (error) => {
      
      this.client.set(null);
      this.loading.set(false);
    }
  });
}

  getBillingModeLabel(mode?: number): string {
    if (mode === undefined) return 'Não informado';
    return BillingModeLabels[mode as keyof typeof BillingModeLabels] || 'Desconhecido';
  }

  editClient() {
    if (this.client()) {
      this.router.navigate(['/clients', this.client()!.clientId, 'edit']);
    }
  }

  goBack() {
    this.router.navigate(['/clients']);
  }
  // ... existing code ...
getStatusClass(status?: string): string {
  if (!status) return 'status-unknown';
  return `status-${status.toLowerCase()}`;
}

getStatusLabel(status?: string): string {
  return status || 'N/A';
}
// ... existing code ...
}