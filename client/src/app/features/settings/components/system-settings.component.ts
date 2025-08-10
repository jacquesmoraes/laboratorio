import { Component, OnInit, inject, signal, ChangeDetectionStrategy, DestroyRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatDividerModule } from '@angular/material/divider';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ErrorService } from '../../../core/services/error.service';
import { SystemSettings, UpdateSystemSettingsDto } from '../models/system.interface';
import { SettingsService } from '../services/system-settings.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import Swal from 'sweetalert2';


@Component({
  selector: 'app-settings',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule,
    MatDividerModule,
    MatProgressSpinnerModule
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './system-settings.component.html',
  styleUrls: ['./system-settings.component.scss']
})
export class SystemSettingsComponent implements OnInit {
  private readonly settingsService = inject(SettingsService);
    private readonly fb = inject(FormBuilder);
    private readonly destroyRef = inject(DestroyRef);

  protected readonly settings = signal<SystemSettings | null>(null);
  protected readonly isLoading = signal<boolean>(false);
  protected readonly isUploadingLogo = signal<boolean>(false);

  protected readonly settingsForm = this.fb.group({
    labName: ['', [Validators.required, Validators.minLength(2)]],
    email: ['', [Validators.required, Validators.email]],
    phone: ['', [Validators.required]],
    cnpj: ['', [Validators.required]],
    footerMessage: ['', [Validators.required]],
    address: this.fb.group({
      street: ['', [Validators.required]],
      cep: ['', [Validators.required]],
      number: [0, [Validators.required, Validators.min(1)]],
      complement: [''],
      neighborhood: ['', [Validators.required]],
      city: ['', [Validators.required]]
    })
  });

  ngOnInit(): void {
    this.loadSettings();
  }

  private loadSettings(): void {
    this.isLoading.set(true);
    
    this.settingsService.getSettings()
    .pipe(takeUntilDestroyed(this.destroyRef))
    .subscribe({
      next: (settings) => {
        this.settings.set(settings);
        this.settingsForm.patchValue({
          labName: settings.labName,
          email: settings.email,
          phone: settings.phone,
          cnpj: settings.cnpj,
          footerMessage: settings.footerMessage,
          address: settings.address
        });
        this.isLoading.set(false);
      },
      error: () => {
       
        this.isLoading.set(false);
      }
    });
  }

  protected onSubmit(): void {
    if (this.settingsForm.invalid) return;

    const formValue = this.settingsForm.value;
    const dto: UpdateSystemSettingsDto = {
      labName: formValue.labName!,
      email: formValue.email!,
      phone: formValue.phone!,
      cnpj: formValue.cnpj!,
      footerMessage: formValue.footerMessage!,
      labAddressRecord: {
        street: formValue.address!.street!,
        cep: formValue.address!.cep!,
        number: formValue.address!.number!,
        complement: formValue.address!.complement || '',
        neighborhood: formValue.address!.neighborhood!,
        city: formValue.address!.city!
      }
    };

    this.settingsService.updateSettings(dto)
      .pipe(takeUntilDestroyed(this.destroyRef)) 
      .subscribe({
        next: () => {
         
          Swal.fire({
            icon: 'success',
            title: 'Sucesso!',
            text: 'Configurações atualizadas com sucesso',
            timer: 2000,
            showConfirmButton: false
          });
          this.loadSettings(); // Recarrega para pegar a data de atualização
        },
        error: () => {
         
        }
      });
  }

  protected onLogoUpload(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    
    if (!file) return;

    this.isUploadingLogo.set(true);
    
    this.settingsService.uploadLogo(file)
    .pipe(takeUntilDestroyed(this.destroyRef))
    .subscribe({
      next: (response) => {
       
        Swal.fire({
          icon: 'success',
          title: 'Sucesso!',
          text: 'Logo atualizada com sucesso',
          timer: 2000,
          showConfirmButton: false
        });
        this.loadSettings(); // Recarrega para pegar a nova logo
        this.isUploadingLogo.set(false);
        
        // Limpa o input para permitir upload do mesmo arquivo novamente
        input.value = '';
      },
      error: () => {
       
        this.isUploadingLogo.set(false);
        input.value = '';
      }
    });
  }
}