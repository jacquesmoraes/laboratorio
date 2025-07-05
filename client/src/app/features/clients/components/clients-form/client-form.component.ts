import { Component, OnInit, inject, signal, ChangeDetectionStrategy } from '@angular/core';
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
import { Client, CreateClientDto, UpdateClientDto, BillingMode, BillingModeLabels } from '../../models/client.interface';

import { ClientService } from '../../services/clients.services';

import { ErrorService } from '../../../../core/services/error.service';
import { TablePriceService } from '../../../table-price/services/table-price.services';
import { TablePriceOption } from '../../../table-price/table-price.interface';

@Component({
  selector: 'app-client-form',
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
  templateUrl: './client-form.component.html',
  styleUrl: './client-form.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ClientFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private clientService = inject(ClientService);
  private tablePriceService = inject(TablePriceService);
  private errorService = inject(ErrorService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  
  clientForm!: any;
  loading = signal(false);
  isEditMode = signal(false);
  clientId = signal<number | null>(null);
  tablePriceOptions = signal<TablePriceOption[]>([]);
  loadingTablePrices = signal(false);
  
  billingModes = Object.entries(BillingModeLabels).map(([value, label]) => ({
    value: parseInt(value),
    label
  }));

  ngOnInit() {
    this.initForm();
    this.loadTablePrices();
    this.checkEditMode();
  }

  private checkEditMode() {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode.set(true);
      this.clientId.set(parseInt(id));
      this.loadClient(parseInt(id));
    }
  }

  private loadClient(id: number) {
    this.loading.set(true);
    this.clientService.getClientById(id)
      .subscribe({
        next: (client) => {
          this.populateForm(client);
          this.loading.set(false);
        },
        error: (error) => {
          this.errorService.showError('Erro ao carregar cliente', error);
          this.loading.set(false);
        }
      });
  }

  private loadTablePrices() {
    this.loadingTablePrices.set(true);
    this.tablePriceService.getTablePriceOptions()
      .subscribe({
        next: (options) => {
          this.tablePriceOptions.set(options);
          this.loadingTablePrices.set(false);
        },
        error: (error) => {
          this.errorService.showError('Erro ao carregar tabelas de preços', error);
          this.loadingTablePrices.set(false);
        }
      });
  }

  private initForm() {
    this.clientForm = this.fb.group({
      // Basic info
      name: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.email]],
      phoneNumber: [''],
      cpf: [''],
      cnpj: [''],
      cro: [''],
      birthDate: [''],
      
      // Billing
      billingMode: [BillingMode.PerServiceOrder, Validators.required],
      tablePriceId: [null],
      
      // Address
      address: this.fb.group({
        street: ['', Validators.required],
        number: ['', [Validators.required, Validators.min(1)]],
        complement: [''],
        neighborhood: ['', Validators.required],
        city: ['', Validators.required]
      })
    });
  }

  private populateForm(client: Client) {
    this.clientForm.patchValue({
      name: client.clientName,
      email: client.clientEmail || '',
      phoneNumber: client.clientPhoneNumber || '',
      cpf: client.clientCpf || '',
      cnpj: client.cnpj || '',
      cro: client.cro || '',
      birthDate: client.birthDate || '',
      billingMode: client.billingMode,
      tablePriceId: client.tablePriceId || null,
      address: {
        street: client.address.street,
        number: client.address.number,
        complement: client.address.complement,
        neighborhood: client.address.neighborhood,
        city: client.address.city
      }
    });
  }

  onSubmit() {
    if (this.clientForm.valid) {
      this.loading.set(true);
      
      const formValue = this.clientForm.value;
      
      if (this.isEditMode() && this.clientId()) {
        const updateDto: UpdateClientDto = {
          clientId: this.clientId()!,
          clientName: formValue.name,
          clientEmail: formValue.email || undefined,
          clientPhoneNumber: formValue.phoneNumber || undefined,
          clientCpf: formValue.cpf || undefined,
          billingMode: formValue.billingMode,
          tablePriceId: formValue.tablePriceId || 0,
          address: formValue.address
        };
        
        this.clientService.updateClient(this.clientId()!, updateDto)
          .subscribe({
            next: () => {
              this.router.navigate(['/clients']);
              this.loading.set(false);
            },
            error: (error) => {
              this.errorService.showError('Erro ao atualizar cliente', error);
              this.loading.set(false);
            }
          });
      } else {
        const createDto: CreateClientDto = {
          name: formValue.name,
          email: formValue.email || undefined,
          phoneNumber: formValue.phoneNumber || undefined,
          cpf: formValue.cpf || undefined,
          cnpj: formValue.cnpj || undefined,
          cro: formValue.cro || undefined,
          birthDate: formValue.birthDate || undefined,
          billingMode: formValue.billingMode,
          tablePriceId: formValue.tablePriceId || undefined,
          address: formValue.address
        };
        
        this.clientService.createClient(createDto)
          .subscribe({
            next: () => {
              this.router.navigate(['/clients']);
              this.loading.set(false);
            },
            error: (error) => {
              this.errorService.showError('Erro ao criar cliente', error);
              this.loading.set(false);
            }
          });
      }
    }
  }

  onCancel() {
    this.router.navigate(['/clients']);
  }
}