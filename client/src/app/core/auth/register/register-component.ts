import { Component, signal, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { MatRadioModule } from '@angular/material/radio';

import { AuthService } from '../../../core/services/auth.service';
import { ClientService } from '../../../features/clients/services/clients.service';
import {
  RegisterAdminRequest,
  RegisterClientRequest
} from '../../../core/models/auth.interface';
import { Client } from '../../../features/clients/models/client.interface';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDividerModule } from '@angular/material/divider';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';

@Component({
  selector: 'app-register',
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
    MatSelectModule,
    MatRadioModule,
    MatDividerModule
  ],
  templateUrl: './register-component.html',
  styleUrls: ['./register-component.scss']
})
export class RegisterComponent implements OnInit {
  private authService = inject(AuthService);
  private clientService = inject(ClientService);
  private router = inject(Router);

  userType = signal<'admin' | 'client'>('admin');
  loading = signal(false);
  error = signal('');
  clients = signal<Client[]>([]);

  adminData = signal<RegisterAdminRequest>({
    displayName: '',
    userName: '',
    email: '',
    password: '',
    confirmPassword: ''
  });

  clientData = signal<RegisterClientRequest>({
    clientId: 0,
    displayName: '',
    email: '',
    password: '',
    confirmPassword: ''
  });

  ngOnInit() {
    this.loadClients();
  }

  private loadClients() {
    this.clientService.getClients().subscribe({
      next: (clients) => this.clients.set(clients),
      error: (err) => {
        console.error('Erro ao carregar clientes:', err);
        this.error.set('Erro ao carregar lista de clientes');
      }
    });
  }

  async registerAdmin(): Promise<void> {
    this.loading.set(true);
    this.error.set('');

    const data = this.adminData();
    data.email = data.email.trim().toLowerCase();

    if (!data.displayName || !data.email || !data.password || !data.confirmPassword) {
      this.error.set('Preencha todos os campos obrigatórios');
      this.loading.set(false);
      return;
    }

    if (data.password !== data.confirmPassword) {
      this.error.set('As senhas não coincidem');
      this.loading.set(false);
      return;
    }

    const success = await this.authService.registerAdmin(data);

    this.loading.set(false);

    if (success) {
      this.router.navigate(['/admin']);
    } else {
      this.error.set('Erro ao registrar administrador');
    }
  }

  async registerClient(): Promise<void> {
    this.loading.set(true);
    this.error.set('');

    const data = this.clientData();
    data.email = data.email.trim().toLowerCase();

    if (
      !data.clientId ||
      !data.displayName ||
      !data.email ||
      !data.password ||
      !data.confirmPassword
    ) {
      this.error.set('Preencha todos os campos obrigatórios');
      this.loading.set(false);
      return;
    }

    if (data.clientId <= 0) {
      this.error.set('Selecione um cliente válido');
      this.loading.set(false);
      return;
    }

    if (data.password !== data.confirmPassword) {
      this.error.set('As senhas não coincidem');
      this.loading.set(false);
      return;
    }

    const success = await this.authService.registerClient(data);

    this.loading.set(false);

    if (success) {
      this.router.navigate(['/admin']);
    } else {
      this.error.set('Erro ao registrar cliente');
    }
  }

  goBack(): void {
    this.router.navigate(['/admin']);
  }
}
