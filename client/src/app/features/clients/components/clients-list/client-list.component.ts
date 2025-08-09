import { Component, signal, inject, OnInit, OnDestroy, ChangeDetectionStrategy } from '@angular/core';
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
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { ReactiveFormsModule, FormControl } from '@angular/forms';
import Swal from 'sweetalert2';
import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';
import { Subject } from 'rxjs';

import { Client, BillingMode, BillingModeLabels, Pagination, QueryParams } from '../../models/client.interface';
import { ErrorService } from '../../../../core/services/error.service';
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
    MatPaginatorModule,
    ReactiveFormsModule
  ],
  templateUrl: './client-list.component.html',
  styleUrls: ['./client-list.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ClientListComponent implements OnInit, OnDestroy {
  private clientService = inject(ClientService);
  private errorService = inject(ErrorService);
  private router = inject(Router);
  private destroy$ = new Subject<void>();

  clients = signal<Client[]>([]);
  loading = signal(false);

  searchControl = new FormControl<string>('', { nonNullable: true });

  private readonly defaultParams: QueryParams = {
    pageNumber: 1,
    pageSize: 10,
    sort: 'clientName',
    search: ''
  };
  currentParams = signal<QueryParams>({ ...this.defaultParams });

  pagination = signal<Pagination<Client> | null>(null);
  displayedColumns = ['clientName', 'city', 'billingMode', 'isInactive', 'actions'];
  billingModeLabels = BillingModeLabels;

  ngOnInit() {
    this.loadClients();
    this.setupSearch();
  }

  private loadClients() {
    this.loading.set(true);
    this.clientService.getPaginatedClients(this.currentParams())
      .subscribe({
        next: pagination => {
          this.pagination.set(pagination);
          this.clients.set(pagination.data);
          this.loading.set(false);
        },
        error: error => {
          this.errorService.showError('Erro ao carregar clientes', error);
          this.loading.set(false);
        }
      });
  }

  private setupSearch() {
    this.searchControl.valueChanges
      .pipe(debounceTime(300), distinctUntilChanged(), takeUntil(this.destroy$))
      .subscribe(search => {
        this.currentParams.update(params => ({ ...params, search, pageNumber: 1 }));
        this.loadClients();
      });
  }

  onPageChange(event: PageEvent) {
    this.currentParams.update(params => ({
      ...params,
      pageNumber: event.pageIndex + 1,
      pageSize: event.pageSize
    }));
    this.loadClients();
  }

  clearFilters() {
    this.searchControl.setValue('');
    this.currentParams.set({ ...this.defaultParams });
    this.loadClients();
  }

  onNew() {
    this.router.navigate(['/clients/new']);
  }

  onEdit(client: Client) {
    this.router.navigate(['/clients', client.clientId, 'edit']);
  }

  onDelete(client: Client) {
    Swal.fire({
      title: 'Tem certeza?',
      text: `Deseja excluir o cliente "${client.clientName}"? Esta ação não pode ser desfeita!`,
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#d33',
      confirmButtonText: 'Sim, excluir!',
      cancelButtonText: 'Cancelar'
    }).then(result => {
      if (result.isConfirmed) {
        this.loading.set(true);
        this.clientService.deleteClient(client.clientId).subscribe({
          next: () => {
            Swal.fire({
              icon: 'success',
              title: 'Sucesso!',
              text: 'Cliente excluído com sucesso',
              timer: 1000,
              showConfirmButton: false
            });
            this.loadClients();
          },
          error: error => {
            this.errorService.showError('Erro ao excluir cliente', error);
            this.loading.set(false);
          }
        });
      }
    });
  }

  onViewDetails(client: Client) {
    this.router.navigate(['/clients', client.clientId]);
  }

  getBillingModeLabel(mode: BillingMode): string {
    return this.billingModeLabels[mode] || 'Desconhecido';
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
