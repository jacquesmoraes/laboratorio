import { Component, inject, signal, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatChipsModule } from '@angular/material/chips';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import Swal from 'sweetalert2';

import { UserManagementService } from '../../services/user-management.service';
import { ClientUserDetailsRecord, LoginHistoryRecord } from '../../models/user-management.interface';
import { ErrorService } from '../../../../core/services/error.service';

@Component({
  selector: 'app-user-details',
  standalone: true,
  imports: [
    CommonModule,
    MatIconModule,
    MatButtonModule,
    MatChipsModule,
    MatCardModule,
    MatProgressSpinnerModule,
    MatTooltipModule
  ],
  templateUrl: './user-details.component.html',
  styleUrl: './user-details.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class UserDetailsComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private userManagementService = inject(UserManagementService);
  private errorService = inject(ErrorService);

  user = signal<ClientUserDetailsRecord | null>(null);
  loading = signal(true);

  ngOnInit() {
  const userId = this.route.snapshot.paramMap.get('userId');

  if (userId) {
    this.loadUser(userId);
  } else {
    console.error('UserId inválido ou não encontrado');
    this.router.navigate(['/user-management']);
  }
}

 private loadUser(userId: string) {
  this.loading.set(true);
  this.userManagementService.getClientUserDetails(userId).subscribe({
    next: (user) => {
      this.user.set(user);
      this.loading.set(false);
    },
    error: (error) => {
      this.errorService.showError('Erro ao carregar detalhes do usuário', error);
      this.user.set(null);
      this.loading.set(false);
    }
  });
}

  getStatusLabel(isActive: boolean): string {
    return isActive ? 'Ativo' : 'Bloqueado';
  }

  getStatusColor(isActive: boolean): string {
    return isActive ? 'accent' : 'warn';
  }

  getAccessCodeStatusLabel(isValid: boolean): string {
    return isValid ? 'Válido' : 'Expirado';
  }

  getAccessCodeStatusColor(isValid: boolean): string {
    return isValid ? 'accent' : 'warn';
  }

  formatDate(date: string | undefined): string {
    if (!date) return '-';
    return new Date(date).toLocaleDateString('pt-BR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  formatAccessCodeExpiry(date: string | undefined): string {
    if (!date) return '-';
    return new Date(date).toLocaleDateString('pt-BR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  async onBlockUser() {
    if (!this.user()) return;

    const result = await Swal.fire({
      title: 'Bloquear Usuário',
      text: `Tem certeza que deseja bloquear o acesso de ${this.user()!.clientName}?`,
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#d33',
      cancelButtonColor: '#3085d6',
      confirmButtonText: 'Sim, bloquear',
      cancelButtonText: 'Cancelar'
    });

    if (result.isConfirmed) {
      this.loading.set(true);
      this.userManagementService.blockClientUser(this.user()!.userId.toString())
        .subscribe({
          next: () => {
            Swal.fire('Sucesso!', 'Usuário bloqueado com sucesso.', 'success');
            this.loadUser(this.user()!.userId);
          },
          error: (error) => {
            this.errorService.showError('Erro ao bloquear usuário', error);
            this.loading.set(false);
          }
        });
    }
  }

  async onUnblockUser() {
    if (!this.user()) return;

    const result = await Swal.fire({
      title: 'Desbloquear Usuário',
      text: `Tem certeza que deseja desbloquear o acesso de ${this.user()!.clientName}?`,
      icon: 'question',
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#6c757d',
      confirmButtonText: 'Sim, desbloquear',
      cancelButtonText: 'Cancelar'
    });

    if (result.isConfirmed) {
      this.loading.set(true);
      this.userManagementService.unblockClientUser(this.user()!.userId.toString())
        .subscribe({
          next: () => {
            Swal.fire('Sucesso!', 'Usuário desbloqueado com sucesso.', 'success');
            this.loadUser(this.user()!.userId);
          },
          error: (error) => {
            this.errorService.showError('Erro ao desbloquear usuário', error);
            this.loading.set(false);
          }
        });
    }
  }

  async onResetAccessCode() {
    if (!this.user()) return;

    const result = await Swal.fire({
      title: 'Resetar Código de Acesso',
      text: `Tem certeza que deseja gerar um novo código de acesso para ${this.user()!.clientName}?`,
      icon: 'question',
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#6c757d',
      confirmButtonText: 'Sim, resetar',
      cancelButtonText: 'Cancelar'
    });

    if (result.isConfirmed) {
      this.loading.set(true);
      this.userManagementService.resetClientUserAccessCode(this.user()!.userId)

        .subscribe({
          next: (response) => {
            Swal.fire({
              title: 'Código Gerado!',
              text: `Novo código de acesso: ${response.accessCode}`,
              icon: 'success',
              confirmButtonText: 'OK'
            });
            this.loadUser(this.user()!.userId);
          },
          error: (error) => {
            this.errorService.showError('Erro ao resetar código de acesso', error);
            this.loading.set(false);
          }
        });
    }
  }

  goBack() {
    this.router.navigate(['/user-management']);
  }

  editUser() {
    if (this.user()) {
      this.router.navigate(['/user-management', this.user()!.userId, 'edit']);
    }
  }
}