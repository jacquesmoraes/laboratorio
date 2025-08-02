import { Component, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { LoginRequest } from '../../../core/models/auth.interface';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';

import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

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
    MatIconModule
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

  async onSubmit(): Promise<void> {
    if (!this.credentials().email || !this.credentials().password) {
      this.error.set('Preencha todos os campos');
      return;
    }

    this.loading.set(true);
    this.error.set('');

    const success = await this.authService.login(this.credentials());
    
    if (success) {
    if (this.authService.isFirstLogin()) {
      this.router.navigate(['/complete-first-access']);
    } else if (this.authService.isClient()) {
      this.router.navigate(['/client-area']); // ← Clientes vão para client-area
    } else {
      this.router.navigate(['/']); // ← Admins vão para dashboard
    }
  }
  
    
    this.loading.set(false);
  }


  
}