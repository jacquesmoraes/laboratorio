import { Component, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { CompleteFirstAccessRequest, LoginRequest } from '../../../core/models/auth.interface';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';

import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatCheckboxModule } from '@angular/material/checkbox';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule,
    FormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatIconModule,
    MatCheckboxModule
  ],
  templateUrl: './login-component.html',
  styleUrls: ['./login-component.scss']
})
export class LoginComponent {
  private authService = inject(AuthService);
  private router = inject(Router);

  credentials = signal<LoginRequest>({
    email: '',
    password: ''
  });

  loading = signal(false);
  error = signal('');
  isFirstAccess = signal(false);
  accessCode = signal('');

  async onSubmit(): Promise<void> {
    if (!this.credentials().email || !this.credentials().password) {
      this.error.set('Preencha todos os campos');
      return;
    }

    if (this.isFirstAccess() && !this.accessCode()) {
      this.error.set('Digite o código de acesso');
      return;
    }

    this.loading.set(true);
    this.error.set('');

    if (this.isFirstAccess()) {
      // Primeiro acesso - usar completeFirstAccess
      const firstAccessData: CompleteFirstAccessRequest = {
        email: this.credentials().email,
        accessCode: this.accessCode(),
        newPassword: this.credentials().password,
        confirmNewPassword: this.credentials().password
      };

      const result = await this.authService.completeFirstAccess(firstAccessData);

      if (result.success) {
        if (this.authService.isClient()) {
          this.router.navigate(['/client-area']);
        } else {
          this.router.navigate(['/']);
        }
      } else {
        this.error.set(result.error || 'Código de acesso inválido ou email não encontrado');
      }
    } else {
      // Login normal
      const result = await this.authService.login(this.credentials());

      if (result.success) {
        if (this.authService.isClient()) {
          this.router.navigate(['/client-area']);
        } else {
          this.router.navigate(['/']);
        }
      } else {
        this.error.set(result.error || 'Email ou senha inválidos');
      }
    }

    this.loading.set(false);
  }

   
  onFirstAccessChange(): void {
    this.error.set('');
  }

}