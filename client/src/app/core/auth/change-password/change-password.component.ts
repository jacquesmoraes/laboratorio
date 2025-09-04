import { Component, signal, inject, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AuthService } from '../../services/auth.service';
import { ErrorMappingService } from '../../services/error.mapping.service';
import { ChangePasswordRequest } from '../../models/auth.interface';
import { PasswordStrengthValidatorComponent } from '../../../shared/components/password-strength-validator';

@Component({
  selector: 'app-change-password',
  standalone: true,
  imports: [
    CommonModule, 
    FormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatIconModule,
    PasswordStrengthValidatorComponent
  ],
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.scss']
})
export class ChangePasswordComponent {
  private authService = inject(AuthService);
  private router = inject(Router);
  private snackBar = inject(MatSnackBar);
  private errorMapping = inject(ErrorMappingService);

  formData = signal<ChangePasswordRequest>({
    currentPassword: '',
    newPassword: '',
    confirmNewPassword: ''
  });
  
  loading = signal(false);
  error = signal('');

  // Adicionar computed para validação em tempo real
  isNewPasswordValid = computed(() => {
    return this.formData().newPassword.length >= 6;
  });

  async onSubmit(): Promise<void> {
    if (!this.formData().currentPassword || !this.formData().newPassword || !this.formData().confirmNewPassword) {
      this.error.set('Preencha todos os campos');
      return;
    }

    if (this.formData().newPassword !== this.formData().confirmNewPassword) {
      this.error.set('As senhas não coincidem');
      return;
    }

    if (this.formData().newPassword.length < 6) {
      this.error.set('A nova senha deve ter pelo menos 6 caracteres');
      return;
    }

    this.loading.set(true);
    this.error.set('');

    try {
      await this.authService.changePassword(this.formData());
      
      this.snackBar.open('Senha alterada com sucesso!', 'Fechar', {
        duration: 3000,
        horizontalPosition: 'center',
        verticalPosition: 'top'
      });
      
      this.goBack();
    } catch (error) {
      this.error.set(this.errorMapping.getAuthErrorMessage(error));
    } finally {
      this.loading.set(false);
    }
  }

  goBack(): void {
    // Verifica se está na área do cliente
    const url = this.router.url;
    if (url.includes('/client-area')) {
      this.router.navigate(['/client-area']);
    } else {
      if (this.authService.isClient()) {
        this.router.navigate(['/client-area']);
      } else {
        this.router.navigate(['/admin']);
      }
    }
  }
}