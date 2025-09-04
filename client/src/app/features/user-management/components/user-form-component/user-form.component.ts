import { Component, OnInit, inject, signal, ChangeDetectionStrategy, DestroyRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatSelectModule } from '@angular/material/select';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDividerModule } from '@angular/material/divider';
import { ActivatedRoute, Router } from '@angular/router';
import Swal from 'sweetalert2';

import { UserManagementService } from '../../services/user-management.service';
import { ClientService } from '../../../clients/services/clients.service';
import { 
  RegisterClientUserRequest, 
  ClientUserRegistrationResponse,
  ClientUserDetailsRecord 
} from '../../models/user-management.interface';
import { Client } from '../../../clients/models/client.interface';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ErrorMappingService } from '../../../../core/services/error.mapping.service';

@Component({
  selector: 'app-user-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule,
    MatSelectModule,
    MatProgressSpinnerModule,
    MatDividerModule
  ],
  templateUrl: './user-form.component.html',
  styleUrl: './user-form.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class UserFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private userManagementService = inject(UserManagementService);
  private clientService = inject(ClientService);
  private readonly destroyRef = inject(DestroyRef);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private errorMapping = inject(ErrorMappingService);

  userForm!: any;
  loading = signal(false);
  isEditMode = signal(false);
  userId = signal<string | null>(null);
  clients = signal<Client[]>([]);
  loadingClients = signal(false);
  userToLoad = signal<ClientUserDetailsRecord | null>(null);
  registrationResponse = signal<ClientUserRegistrationResponse | null>(null);
  

  ngOnInit() {
    this.initForm();
    this.loadClients();
    this.checkEditMode();
  }

  private initForm() {
    this.userForm = this.fb.group({
      clientId: [null, Validators.required],
      displayName: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.required, Validators.email]]
    });
  }

  private checkEditMode() {
    const userId = this.route.snapshot.paramMap.get('userId');
    if (userId) {
      this.isEditMode.set(true);
      this.userId.set(userId);
      this.loadUser(userId);
    }
  }

  private loadUser(userId: string) {
    this.loading.set(true);
    this.userManagementService.getClientUserDetails(userId)
      .pipe(takeUntilDestroyed(this.destroyRef)) 
      .subscribe({
        next: (user) => {
          this.userToLoad.set(user);
          this.populateForm(user);
          this.loading.set(false);
        },
        error: (error) => {
          this.loading.set(false);
        }
      });
  }

  private loadClients() {
    this.loadingClients.set(true);
    this.clientService.getClients()
      .pipe(takeUntilDestroyed(this.destroyRef)) 
      .subscribe({
        next: (clients) => {
          this.clients.set(clients);
          this.loadingClients.set(false);
        },
        error: (error) => {
          this.loadingClients.set(false);
        }
      });
  }

  private populateForm(user: ClientUserDetailsRecord) {
    this.userForm.patchValue({
      clientId: user.clientId,
      displayName: user.clientName,
      email: user.email,
    });

    // Desabilitar campos que não devem ser editados
    this.userForm.get('clientId')?.disable();
    this.userForm.get('email')?.disable();
  }

  onClientChange() {
    const clientId = this.userForm.get('clientId')?.value;
    if (clientId) {
      const selectedClient = this.clients().find(c => c.clientId === clientId);
      if (selectedClient) {
        this.userForm.patchValue({
          displayName: selectedClient.clientName
        });
      }
    }
  }

  onSubmit() {
    if (this.userForm.invalid) {
      this.markFormGroupTouched(this.userForm);
      return;
    }

    this.loading.set(true);

    if (this.isEditMode()) {
      
      Swal.fire({
        icon: 'info',
        title: 'Funcionalidade em Desenvolvimento',
        text: 'Edição de usuários ainda não implementada',
        confirmButtonText: 'OK'
      });
      this.loading.set(false);
    } else {
      const formValue = this.userForm.value;
      const request: RegisterClientUserRequest = {
        clientId: formValue.clientId,
        displayName: formValue.displayName,
        email: formValue.email
      };

      this.userManagementService.registerClientUser(request)
        .pipe(takeUntilDestroyed(this.destroyRef)) 
        .subscribe({
          next: (response) => {
            this.registrationResponse.set(response);
            this.showSuccessDialog(response);
            this.loading.set(false);
          },
          error: (error) => {
            this.loading.set(false);
            this.showErrorDialog(error);
          }
        });
    }
  }

  private showSuccessDialog(response: ClientUserRegistrationResponse) {
    const userInfo = `Cliente: ${response.user.displayName}
Email: ${response.user.email}
Código de Acesso: ${response.user.accessCode}
Expira em: ${new Date(response.expiresAt).toLocaleString('pt-BR')}`;

    Swal.fire({
      title: 'Usuário Registrado com Sucesso!',
      html: `
        <div class="success-dialog">
          <p><strong>Cliente:</strong> ${response.user.displayName}</p>
          <p><strong>Email:</strong> ${response.user.email}</p>
          <p><strong>Código de Acesso:</strong></p>
          <div class="access-code-display">
            <span class="access-code">${response.user.accessCode}</span>
          </div>
          <p class="access-code-info">
            <small>Este código expira em: ${new Date(response.expiresAt).toLocaleString('pt-BR')}</small>
          </p>
          <p class="warning">
            <strong>⚠️ Importante:</strong> Informe este código ao usuário para que ele possa completar o primeiro acesso.
          </p>
        </div>
      `,
      icon: 'success',
      showCancelButton: true,
      confirmButtonText: 'Copiar Informações',
      cancelButtonText: 'OK',
      confirmButtonColor: '#276678',
      cancelButtonColor: '#96afb8',
      reverseButtons: true
    }).then((result) => {
      if (result.isConfirmed) {
        // Copiar informações para a área de transferência
        navigator.clipboard.writeText(userInfo).then(() => {
          Swal.fire({
            title: 'Copiado!',
            text: 'As informações foram copiadas para a área de transferência',
            icon: 'success',
            timer: 2000,
            showConfirmButton: false
          });
        }).catch(() => {
          // Fallback para navegadores que não suportam clipboard API
          this.fallbackCopyTextToClipboard(userInfo);
        });
      }
      // this.router.navigate(['/admin/user-management']);
    });
  }

  private fallbackCopyTextToClipboard(text: string) {
    const textArea = document.createElement('textarea');
    textArea.value = text;
    textArea.style.position = 'fixed';
    textArea.style.left = '-999999px';
    textArea.style.top = '-999999px';
    document.body.appendChild(textArea);
    textArea.focus();
    textArea.select();
    
    try {
      document.execCommand('copy');
      Swal.fire({
        title: 'Copiado!',
        text: 'As informações foram copiadas para a área de transferência',
        icon: 'success',
        timer: 2000,
        showConfirmButton: false
      });
    } catch (err) {
      Swal.fire({
        title: 'Erro',
        text: 'Não foi possível copiar as informações automaticamente',
        icon: 'error',
        confirmButtonColor: '#276678'
      });
    }
    
    document.body.removeChild(textArea);
  }

  private showErrorDialog(error: any) {
    const errorMessage = this.errorMapping.getAuthErrorMessage(error);
    Swal.fire({
      icon: 'error',
      title: 'Erro no Registro',
      text: errorMessage,
      confirmButtonText: 'OK',
      confirmButtonColor: '#276678'
    });
  }

  onCancel() {
    this.router.navigate(['/admin/user-management']);
  }

  private markFormGroupTouched(formGroup: any) {
    Object.keys(formGroup.controls).forEach(key => {
      const control = formGroup.controls[key];
      control.markAsTouched();

      if (control.controls) {
        this.markFormGroupTouched(control);
      }
    });
  }

  getErrorMessage(controlName: string): string {
    const control = this.userForm.get(controlName);
    
    if (control?.hasError('required')) {
      return `${this.getFieldLabel(controlName)} é obrigatório`;
    }
    
    if (control?.hasError('email')) {
      return 'Email inválido';
    }
    
    if (control?.hasError('minlength')) {
      const requiredLength = control.errors?.['minlength']?.requiredLength;
      return `${this.getFieldLabel(controlName)} deve ter pelo menos ${requiredLength} caracteres`;
    }
    
    if (control?.hasError('passwordMismatch')) {
      return 'As senhas não coincidem';
    }
    
    return '';
  }

  private getFieldLabel(fieldName: string): string {
    const labels: { [key: string]: string } = {
      clientId: 'Cliente',
      displayName: 'Nome',
      email: 'Email',
      password: 'Senha',
      confirmPassword: 'Confirmação de senha'
    };
    
    return labels[fieldName] || fieldName;
  }
}