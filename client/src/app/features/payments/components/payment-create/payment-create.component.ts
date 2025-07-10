import { Component, OnInit, signal, inject, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { PaymentService } from '../../services/payment.service';
import { CreatePaymentDto } from '../../models/payment.interface';
import { ClientService } from '../../../clients/services/clients.services';
import { Client } from '../../../clients/models/client.interface';

@Component({
  selector: 'app-payment-create',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './payment-create.component.html',
  styleUrls: ['./payment-create.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class PaymentCreateComponent implements OnInit {
  private paymentService = inject(PaymentService);
  private clientService = inject(ClientService);
  private router = inject(Router);
  private fb = inject(FormBuilder);

  loading = signal(false);
  loadingClients = signal(false);
  paymentForm!: FormGroup;
  clients = signal<Client[]>([]);

  ngOnInit(): void {
    this.initForm();
    this.loadClients();
  }

  private initForm(): void {
    this.paymentForm = this.fb.group({
      clientId: ['', [Validators.required, Validators.min(1)]],
      amountPaid: ['', [Validators.required, Validators.min(0.01)]],
      description: [''],
      paymentDate: [this.formatDateTimeLocal(new Date()), [Validators.required]]
    });
  }

  private loadClients(): void {
    this.loadingClients.set(true);
    this.clientService.getClients().subscribe({
      next: (clients) => {
        this.clients.set(clients);
        this.loadingClients.set(false);
      },
      error: (error) => {
        console.error('Erro ao carregar clientes:', error);
        this.loadingClients.set(false);
      }
    });
  }

  onSubmit(): void {
    if (this.paymentForm.valid) {
      this.loading.set(true);
      
      const paymentData: CreatePaymentDto = {
        clientId: this.paymentForm.value.clientId,
        amountPaid: this.paymentForm.value.amountPaid,
        description: this.paymentForm.value.description,
        paymentDate: this.formatDateForApi(this.paymentForm.value.paymentDate)
      };

      this.paymentService.createPayment(paymentData).subscribe({
        next: () => {
          this.router.navigate(['/payments']);
        },
        error: (error) => {
          console.error('Erro ao criar pagamento:', error);
          this.loading.set(false);
        }
      });
    }
  }

  goBack(): void {
    this.router.navigate(['/payments']);
  }

  private formatDateTimeLocal(date: Date): string {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    const hours = String(date.getHours()).padStart(2, '0');
    const minutes = String(date.getMinutes()).padStart(2, '0');
    
    return `${year}-${month}-${day}T${hours}:${minutes}`;
  }

  private formatDateForApi(dateTimeLocal: string): string {
    return new Date(dateTimeLocal).toISOString();
  }
}