import { Component, signal, computed, inject, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatChipsModule } from '@angular/material/chips';
import { MatTooltipModule } from '@angular/material/tooltip';
import { ReactiveFormsModule, FormControl } from '@angular/forms';

import { Client, BillingMode, BillingModeLabels } from '../../models/client.interface';
import { ErrorService } from '../../../../core/services/error.service';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { ClientService } from '../../services/clients.services';

@Component({
  selector: 'app-client-list',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatProgressSpinnerModule,
    MatChipsModule,
    MatTooltipModule,
    ReactiveFormsModule
  ],
  templateUrl: './client-list.component.html',
  styleUrl: './client-list.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ClientListComponent implements OnInit {
  private clientService = inject(ClientService);
  private errorService = inject(ErrorService);
  private router = inject(Router);
  
  clients = signal<Client[]>([]);
  loading = signal(false);
  searchControl = new FormControl('');
  
  displayedColumns = ['clientName', 'city', 'billingMode', 'tablePriceName', 'isInactive', 'actions'];
  
  filteredClients = computed(() => {
    const searchTerm = this.searchControl.value?.toLowerCase() || '';
    return this.clients().filter(client => 
      client.clientName.toLowerCase().includes(searchTerm) ||
      (client.city && client.city.toLowerCase().includes(searchTerm)) ||
      (client.clientPhoneNumber && client.clientPhoneNumber.includes(searchTerm))
    );
  });

  billingModeLabels = BillingModeLabels;

  ngOnInit() {
    this.loadClients();
    this.setupSearch();
  }

  private loadClients() {
    this.loading.set(true);
    this.clientService.getClients()
      .subscribe({
        next: (clients) => {
          this.clients.set(clients);
          this.loading.set(false);
        },
        error: (error) => {
          this.errorService.showError('Erro ao carregar clientes', error);
          this.loading.set(false);
        }
      });
  }

  private setupSearch() {
    this.searchControl.valueChanges.pipe(
      debounceTime(300),
      distinctUntilChanged()
    ).subscribe(() => {
      // The computed signal will automatically update the filtered results
    });
  }

  onNew() {
    this.router.navigate(['/clients/new']);
  }

  onEdit(client: Client) {
    this.router.navigate(['/clients', client.clientId, 'edit']);
  }

  onDelete(client: Client) {
    // TODO: Implement delete confirmation
    console.log('Delete client:', client);
  }

  onViewDetails(client: Client) {
    this.router.navigate(['/clients', client.clientId]);
  }

  getBillingModeLabel(mode: BillingMode): string {
    return this.billingModeLabels[mode] || 'Desconhecido';
  }
}