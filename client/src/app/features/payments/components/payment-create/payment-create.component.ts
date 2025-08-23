import { Component, OnInit, signal, inject, ChangeDetectionStrategy, DestroyRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators, FormControl } from '@angular/forms';
import { PaymentService } from '../../services/payment.service';
import { CreatePaymentDto } from '../../models/payment.interface';
import { ClientService } from '../../../clients/services/clients.service';
import { Client } from '../../../clients/models/client.interface';

import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import {  MatCardModule } from "@angular/material/card";

@Component({
  selector: 'app-payment-create',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatCardModule,
    MatSelectModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    
],
  templateUrl: './payment-create.component.html',
  styleUrls: ['./payment-create.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class PaymentCreateComponent implements OnInit {
  private paymentService = inject(PaymentService);
  private clientService = inject(ClientService);
  private router = inject(Router);
  private fb = inject(FormBuilder);
  private readonly destroyRef = inject(DestroyRef);
  loading = signal(false);
  loadingClients = signal(false);
  clients = signal<Client[]>([]);

  paymentForm!: FormGroup<{
    clientId: FormControl<number | null>;
    amountPaid: FormControl<number | null>;
    description: FormControl<string | null>;
    paymentDate: FormControl<string>;
  }>;

  ngOnInit(): void {
    this.initForm();
    this.loadClients();
  }

  private initForm(): void {
    this.paymentForm = this.fb.group({
      clientId: this.fb.control<number | null>(null, {
        validators: [Validators.required, Validators.min(1)]
      }),
      amountPaid: this.fb.control<number | null>(null, {
        validators: [Validators.required, Validators.min(0.01)]
      }),
      description: this.fb.control<string | null>(null),
      paymentDate: this.fb.control<string>(
        this.formatDateTimeLocal(new Date()),
        { validators: [Validators.required], nonNullable: true }
      )
    });
  }

  private loadClients(): void {
    this.loadingClients.set(true);
    this.clientService.getClients()
    .pipe(takeUntilDestroyed(this.destroyRef))
    .subscribe({
      next: (clients) => {
        this.clients.set(clients);
        this.loadingClients.set(false);
      },
      error: () => {
        this.loadingClients.set(false);
      }
    });
  }

  onSubmit(): void {
    if (this.paymentForm.valid) {
      this.loading.set(true);

      const formValue = this.paymentForm.getRawValue();
      const paymentData: CreatePaymentDto = {
        clientId: formValue.clientId!,
        amountPaid: formValue.amountPaid!,
        description: formValue.description || undefined,
        paymentDate: this.formatDateForApi(formValue.paymentDate)
      };

      this.paymentService.createPayment(paymentData)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => this.router.navigate(['/admin/payments']),
        error: () => {
          this.loading.set(false);
        }
      });
    }
  }

  goBack(): void {
    this.router.navigate(['/admin/payments']);
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
