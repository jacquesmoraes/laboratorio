import { Component, OnInit, inject, signal, ChangeDetectionStrategy, DestroyRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators, FormGroup } from '@angular/forms';
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

import { ClientService } from '../../services/clients.service';

import { TablePriceService } from '../../../table-price/services/table-price.services';
import { TablePriceOption } from '../../../table-price/table-price.interface';
import Swal from 'sweetalert2';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CepService } from '../../../../core/services/cep.service';
import { ErrorService } from '../../../../core/services/error.service';
import { ErrorMappingService } from '../../../../core/services/error.mapping.service';

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
  private cepService = inject(CepService);
  private tablePriceService = inject(TablePriceService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);
  private errorService = inject(ErrorService);
  private errorMapping = inject(ErrorMappingService);

  clientForm!: FormGroup;
  loading = signal(false);
  isEditMode = signal(false);
  clientId = signal<number | null>(null);
  tablePriceOptions = signal<TablePriceOption[]>([]);
  loadingTablePrices = signal(false);
  clientToLoad = signal<Client | null>(null);
  
  billingModes = Object.entries(BillingModeLabels).map(([value, label]) => ({
    value: parseInt(value),
    label
  }));

  ngOnInit() {
    this.initForm();
    this.loadTablePrices();
    this.checkEditMode();
  }

  private initForm() {
    this.clientForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.email]],
      phoneNumber: [''],
      cpf: [''],
      cnpj: [''],
      cro: [''],
      birthDate: [''],
      notes: [''],
      billingMode: [BillingMode.PerServiceOrder, Validators.required],
      tablePriceId: [null],
      address: this.fb.group({
        street: ['', Validators.required],
        number: ['', [Validators.required, Validators.min(1)]],
        complement: [''],
        cep: [''],
        neighborhood: ['', Validators.required],
        city: ['', Validators.required]
      })
    });

    // Adiciona listener para busca automática de CEP
    this.clientForm.get('address.cep')?.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(cep => {
        if (cep && cep.replace(/\D/g, '').length === 8) {
          this.searchCep(cep);
        }
      });
  }

  searchCep(cep: string) {
    this.cepService.searchCep(cep)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (response) => {
          if (response && !response.erro) {
            this.clientForm.patchValue({
              address: {
                street: response.logradouro,
                neighborhood: response.bairro,
                city: response.localidade,
                complement: response.complemento || ''
              }
            });
          } else {
            // CEP não encontrado
            this.errorService.showError('CEP não encontrado. Verifique se está correto.');
          }
        },
        error: (error) => {
          // Tratar diferentes tipos de erro
          let errorMessage = 'Erro ao buscar CEP';
          
          if (error.status === 404) {
            errorMessage = 'CEP não encontrado. Verifique se está correto.';
          } else if (error.status === 400) {
            errorMessage = error.error?.message || 'CEP inválido';
          } else if (error.message) {
            errorMessage = error.message;
          }
          
          this.errorService.showError(errorMessage);
        }
      });
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
    .pipe(takeUntilDestroyed(this.destroyRef))
    .subscribe({
      next: (client) => {
        this.clientToLoad.set(client);
        this.tryPopulateForm();
        this.loading.set(false);
      },
      error: (error) => {
        
        this.loading.set(false);
      }
    });
  }

  private loadTablePrices() {
    this.loadingTablePrices.set(true);
    this.tablePriceService.getTablePriceOptions()
    .pipe(takeUntilDestroyed(this.destroyRef))
    .subscribe({
      next: (options) => {
        this.tablePriceOptions.set(options);
        this.loadingTablePrices.set(false);
        this.tryPopulateForm();
      },
      error: (error) => {
        
        this.loadingTablePrices.set(false);
      }
    });
  }

  private tryPopulateForm() {
    if (this.clientToLoad() && !this.loadingTablePrices()) {
      this.populateForm(this.clientToLoad()!);
    }
  }

  private populateForm(client: Client) {
    this.clientForm.patchValue({
      name: client.clientName,
      email: client.clientEmail || '',
      phoneNumber: client.clientPhoneNumber || '',
      cpf: client.clientCpf || '',
      cnpj: client.cnpj || '',
      cro: client.cro || '',
      birthDate: client.birthDate ? new Date(client.birthDate).toISOString().substring(0, 10) : '',
      notes: client.notes || '',
      billingMode: client.billingMode,
      tablePriceId: client.tablePriceId ?? null,
      address: {
        street: client.address.street,
        number: client.address.number,
        complement: client.address.complement,
        cep: client.address.cep || '',
        neighborhood: client.address.neighborhood,
        city: client.address.city
      }
    });
  }

  private showSuccess(message: string) {
    Swal.fire({
      icon: 'success',
      title: 'Sucesso!',
      text: message,
      timer: 1000,
      showConfirmButton: false
    }).then(() => {
      this.router.navigate(['/admin/clients']);
    });
  }

  onSubmit() {
    if (!this.clientForm.valid) return;

    this.loading.set(true);
    const formValue = this.clientForm.value;

    if (this.isEditMode() && this.clientId()) {
      const updateDto: UpdateClientDto = {
        clientId: this.clientId()!,
        clientName: formValue.name,
        clientEmail: formValue.email || undefined,
        clientPhoneNumber: formValue.phoneNumber || undefined,
        clientCpf: formValue.cpf || undefined,
        cro: formValue.cro || undefined,
        cnpj: formValue.cnpj || undefined,
        birthDate: formValue.birthDate || undefined,
        notes: formValue.notes || undefined,
        billingMode: formValue.billingMode,
        tablePriceId: formValue.tablePriceId || 0,
        address: formValue.address
      };
      
      this.clientService.updateClient(this.clientId()!, updateDto).subscribe({
        next: () => {
          this.showSuccess('Cliente atualizado com sucesso');
          this.loading.set(false);
        },
        error: (error) => {
          
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
      
      this.clientService.createClient(createDto).subscribe({
        next: () => {
          this.showSuccess('Cliente criado com sucesso');
          this.loading.set(false);
        },
        error: (error) => {
          
          this.loading.set(false);
        }
      });
    }
  }

  onCancel() {
    if (this.clientForm.dirty) {
      Swal.fire({
        title: 'Tem certeza?',
        text: 'Você tem alterações não salvas. Deseja realmente sair?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Sim, sair',
        cancelButtonText: 'Cancelar'
      }).then((result) => {
        if (result.isConfirmed) {
          this.router.navigate(['/admin/clients']);
        }
      });
    } else {
      this.router.navigate(['/admin/clients']);
    }
  }
}
