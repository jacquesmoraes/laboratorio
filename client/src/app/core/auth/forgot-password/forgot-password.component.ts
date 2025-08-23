import { Component, signal, inject, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { ForgotPasswordRequest } from '../../models/auth.interface';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatIconModule
  ],
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ForgotPasswordComponent {
  private authService = inject(AuthService);
  private router = inject(Router);

  email = signal('');
  loading = signal(false);
  error = signal('');
  success = signal('');

  async onSubmit(): Promise<void> {
    if (!this.email()) {
      this.error.set('Digite seu email');
      return;
    }

    this.loading.set(true);
    this.error.set('');
    this.success.set('');

    try {
      this.authService.forgotPassword({ email: this.email() }).subscribe({
        next: (response) => {
          this.success.set(response.message);
          this.loading.set(false);
        },
        error: (error) => {
          this.error.set('Erro ao enviar email de recuperação');
          this.loading.set(false);
        }
      });
    } catch (error) {
      this.error.set('Erro inesperado');
      this.loading.set(false);
    }
  }

  goToLogin(): void {
    this.router.navigate(['/login']);
  }
}