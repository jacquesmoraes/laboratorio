import { Component, signal, inject, OnInit, ChangeDetectionStrategy, DestroyRef } from '@angular/core';
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
import { ReactiveFormsModule, FormControl, FormsModule } from '@angular/forms';
import Swal, { SweetAlertResult } from 'sweetalert2';

import { 
  ClientUserListRecord, 
  UserManagementQueryParams, 
  UserManagementPagination 
} from '../../models/user-management.interface';
import { UserManagementService } from '../../services/user-management.service';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-user-list',
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
    ReactiveFormsModule,
    FormsModule
  ],
  templateUrl: './user-list.component.html',
  styleUrl: './user-list.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class UserListComponent implements OnInit {
  private userManagementService = inject(UserManagementService);
  private router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);
  
  users = signal<ClientUserListRecord[]>([]);
  loading = signal(false);
  
  // Filtros inline
  searchFilter = signal('');
  private searchDebounceTimeout?: number;
  
  // Pagination
  pagination = signal<UserManagementPagination<ClientUserListRecord> | null>(null);
  currentParams = signal<UserManagementQueryParams>({
    pageNumber: 1,
    pageSize: 10,
    sort: 'clientName',
    search: ''
  });

  ngOnInit() {
    this.loadUsers();
  }

  private loadUsers() {
    this.loading.set(true);
    this.userManagementService.getClientUsers(this.currentParams())
      .pipe(takeUntilDestroyed(this.destroyRef)) 
      .subscribe({
        next: (pagination) => {
          this.pagination.set(pagination);
          this.users.set(pagination.data);
          this.loading.set(false);
        },
        error: (error) => {
          this.loading.set(false);
        }
      });
  }

  // Métodos para os filtros inline com debounce
  onSearchChange(value: string) {
    this.searchFilter.set(value);
    
    // Cancelar timeout anterior
    if (this.searchDebounceTimeout) {
      clearTimeout(this.searchDebounceTimeout);
    }
    
    // Criar novo timeout para debounce
    this.searchDebounceTimeout = window.setTimeout(() => {
      this.updateFilters();
    }, 300);
  }

  clearFilters() {
    this.searchFilter.set('');
    
    // Cancelar timeout de busca se existir
    if (this.searchDebounceTimeout) {
      clearTimeout(this.searchDebounceTimeout);
    }
    
    this.updateFilters();
  }

  private updateFilters() {
    this.currentParams.update(params => ({ 
      ...params, 
      search: this.searchFilter(), 
      pageNumber: 1 
    }));
    this.loadUsers();
  }

  onPageChange(event: PageEvent) {
    this.currentParams.update(params => ({
      ...params,
      pageNumber: event.pageIndex + 1,
      pageSize: event.pageSize
    }));
    this.loadUsers();
  }

  onNew() {
    this.router.navigate(['/admin/user-management/new']);
  }

  onViewDetails(user: ClientUserListRecord) {
    this.router.navigate(['/admin/user-management', user.userId]);
  }

  async onBlock(user: ClientUserListRecord) {
    const result:SweetAlertResult = await Swal.fire({
      title: 'Bloquear Usuário',
      text: `Tem certeza que deseja bloquear o acesso de ${user.clientName}?`,
      icon: 'question',
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#6c757d',
      confirmButtonText: 'Sim, bloquear',
      cancelButtonText: 'Cancelar'
    });

    if (result.isConfirmed) {
      this.loading.set(true);
      this.userManagementService.blockClientUser(user.userId)
        .pipe(takeUntilDestroyed(this.destroyRef))
        .subscribe({
          next: () => {
            Swal.fire('Sucesso!', 'Usuário bloqueado com sucesso.', 'success');
            this.loadUsers();
          },
          error: () => {
            this.loading.set(false);
          }
        });
    }
  }

  async onUnblock(user: ClientUserListRecord) {
    const result: SweetAlertResult = await Swal.fire({
      title: 'Desbloquear Usuário',
      text: `Tem certeza que deseja desbloquear o acesso de ${user.clientName}?`,
      icon: 'question',
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#6c757d',
      confirmButtonText: 'Sim, desbloquear',
      cancelButtonText: 'Cancelar'
    });

    if (result.isConfirmed) {
      this.loading.set(true);
      this.userManagementService.unblockClientUser(user.userId)
        .pipe(takeUntilDestroyed(this.destroyRef))
        .subscribe({
          next: () => {
            Swal.fire('Sucesso!', 'Usuário desbloqueado com sucesso.', 'success');
            this.loadUsers();
          },
          error: (error) => {
            this.loading.set(false);
          }
        });
    }
  }

  async onResetAccessCode(user: ClientUserListRecord) {
    const result: SweetAlertResult = await Swal.fire({
      title: 'Resetar Código de Acesso',
      text: `Tem certeza que deseja gerar um novo código de acesso para ${user.clientName}?`,
      icon: 'question',
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#6c757d',
      confirmButtonText: 'Sim, resetar',
      cancelButtonText: 'Cancelar'
    });

    if (result.isConfirmed) {
      this.loading.set(true);
      this.userManagementService.resetClientUserAccessCode(user.userId)
        .pipe(takeUntilDestroyed(this.destroyRef))
        .subscribe({
          next: (response) => {
            Swal.fire({
              title: 'Código Gerado!',
              text: `Novo código de acesso: ${response.accessCode}`,
              icon: 'success',
              confirmButtonText: 'OK'
            });
            this.loadUsers();
          },
          error: (error) => {
            this.loading.set(false);
          }
        });
    }
  }

  getStatusLabel(isActive: boolean): string {
    return isActive ? 'Ativo' : 'Bloqueado';
  }

  getStatusColor(isActive: boolean): string {
    return isActive ? 'accent' : 'warn';
  }

  getAccessCodeStatusLabel(hasValidAccessCode: boolean): string {
    return hasValidAccessCode ? 'Válido' : 'Expirado';
  }

  getAccessCodeStatusColor(hasValidAccessCode: boolean): string {
    return hasValidAccessCode ? 'accent' : 'warn';
  }

  formatDate(date: string | undefined): string {
    if (!date) return '-';
    return new Date(date).toLocaleDateString('pt-BR');
  }
}