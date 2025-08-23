import { Component, signal, inject, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AuthService } from '../../services/auth.service';
import { ResetPasswordRequest } from '../../models/auth.interface';



@Component({
  selector: 'app-reset-password',
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
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ResetPasswordComponent implements OnInit {
  private authService = inject(AuthService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  token = signal('');
  email = signal('');
  newPassword = signal('');
  confirmPassword = signal('');
  loading = signal(false);
  error = signal('');
  success = signal('');

  ngOnInit(): void {
    const token = this.route.snapshot.queryParamMap.get('token');
    const email = this.route.snapshot.queryParamMap.get('email');
    
    if (token) this.token.set(token);
    if (email) this.email.set(email);
  }

  async onSubmit(): Promise<void> {
    if(!this.token() || !this.email() || !this.newPassword()) {
      this.error.set('Todos os campos são obrigatórios');
      return;
    }

    if (this.newPassword() !== this.confirmPassword()) {
      this.error.set('As senhas não coincidem');
      return;
    }

    if (this.newPassword().length < 6) {
      this.error.set('A senha deve ter pelo menos 6 caracteres');
      return;
    }

    this.loading.set(true);
    this.error.set('');
    this.success.set('');

    const resetData: ResetPasswordRequest = {
      token: this.token(),
      email: this.email(),
      newPassword: this.newPassword()
    };

    try {
      this.authService.resetPassword(resetData).subscribe({
        next: (response) => {
          this.success.set(response.message);
          this.loading.set(false);
          // Redirecionar para login após 3 segundos
          setTimeout(() => {
            this.router.navigate(['/login']);
          }, 3000);
        },
        error: (error) => {
          this.error.set('Erro ao redefinir senha. Verifique o token e tente novamente.');
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