import { Component, signal, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';
import { AuthService } from '../../../core/services/auth.service';
import { CompleteFirstAccessRequest } from '../../../core/models/auth.interface';

@Component({
  selector: 'app-complete-first-access',
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
  templateUrl: './first-access-component.html',
  styleUrls: ['./first-access-component.scss']
})
export class FirstAccessComponent implements OnInit {
  private authService = inject(AuthService);
  private router = inject(Router);

  formData = signal<CompleteFirstAccessRequest>({
    email: '',
    accessCode: '',
    newPassword: '',
    confirmNewPassword: ''
  });
  
  loading = signal(false);
  error = signal('');

  ngOnInit() {
    // Preencher email automaticamente se disponível
    const user = this.authService.user();
    if (user?.email) {
      this.formData.update(data => ({ ...data, email: user.email }));
    }
    
    // Preencher código de acesso se disponível
    const accessCode = this.authService.getAccessCode();
    if (accessCode) {
      this.formData.update(data => ({ ...data, accessCode }));
    }
  }

  async onSubmit(): Promise<void> {
    const data = this.formData();
    
    if (!data.email || !data.accessCode || !data.newPassword || !data.confirmNewPassword) {
      this.error.set('Preencha todos os campos');
      return;
    }

    if (data.newPassword !== data.confirmNewPassword) {
      this.error.set('As senhas não coincidem');
      return;
    }

    if (data.newPassword.length < 6) {
      this.error.set('A senha deve ter pelo menos 6 caracteres');
      return;
    }

    this.loading.set(true);
    this.error.set('');

    const success = await this.authService.completeFirstAccess(data);
    
    if (success) {
      if (this.authService.isClient()) {
        this.router.navigate(['/client-area']);
      } else {
        this.router.navigate(['/admin']);
      }
    } else {
      this.error.set('Erro ao completar primeiro acesso. Verifique o código de acesso.');
    }
    
    this.loading.set(false);
  }
}