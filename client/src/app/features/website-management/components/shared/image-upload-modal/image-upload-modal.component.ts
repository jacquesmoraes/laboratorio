import { Component, signal, inject, ChangeDetectionStrategy, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatSnackBar } from '@angular/material/snack-bar';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { UploadProgress } from '../../../models/upload-image.interface';
import { CreateWebsiteCaseImageDto } from '../../../models/website-case.interface';
import { ModalService } from '../services/modal.service';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { ErrorMappingService } from '../../../../../core/services/error.mapping.service';
import Swal from 'sweetalert2';



export interface ImageUploadData {
  imageUrl?: string;
  altText?: string;
  caption?: string;
  order?: number;
  isMainImage?: boolean;
}

@Component({
  selector: 'app-image-upload-modal',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatCheckboxModule,
    MatProgressBarModule
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="image-upload-modal">
      <!-- Header -->
      <div class="modal-header">
        <h2 class="modal-title">
          {{ isEditMode() ? 'Editar' : 'Adicionar' }} Imagem
        </h2>
        <button 
          mat-icon-button 
          (click)="close()"
          class="close-button">
          <mat-icon>close</mat-icon>
        </button>
      </div>

      <!-- Content -->
      <div class="modal-content">
        <form [formGroup]="imageForm" (ngSubmit)="onSubmit()" class="form-container">
          <!-- Área de Upload -->
          <div class="upload-area">
            <mat-icon class="upload-icon">cloud_upload</mat-icon>
            <p class="upload-text">Faça upload de uma imagem ou insira uma URL</p>
            
            <!-- Input de arquivo completamente oculto -->
            <input 
              type="file" 
              #fileInput
              (change)="onFileSelected($event)"
              accept="image/*"
              style="position: absolute; left: -9999px; opacity: 0; pointer-events: none;">
            
            <!-- Botão de seleção de arquivo -->
            <button 
              type="button"
              mat-raised-button 
              color="accent"
              (click)="fileInput.click()"
              [disabled]="uploading()"
              class="select-file-btn">
              <mat-icon>upload_file</mat-icon>
              Selecionar Arquivo
            </button>

            <!-- Informações do arquivo selecionado -->
            @if (selectedFile()) {
              <div class="file-info">
                <p class="file-name">
                  <strong>Arquivo selecionado:</strong> {{ selectedFile()?.name }}
                </p>
                <p class="file-size">
                  Tamanho: {{ (selectedFile()?.size || 0) / 1024 / 1024 | number:'1.2-2' }} MB
                </p>
                
                @if (!uploading()) {
                  <button 
                    type="button"
                    mat-raised-button 
                    color="primary"
                    (click)="uploadFile()"
                    class="upload-btn">
                    <mat-icon>cloud_upload</mat-icon>
                    Fazer Upload
                  </button>
                }
              </div>
            }

            <!-- Progresso do upload -->
            @if (uploading()) {
              <div class="upload-progress">
                <mat-progress-bar 
                  mode="determinate" 
                  [value]="uploadProgress()"
                  class="progress-bar">
                </mat-progress-bar>
                <p class="progress-text">
                  Fazendo upload... {{ uploadProgress() }}%
                </p>
              </div>
            }

            <!-- Separador -->
            <div class="separator">
              <div class="separator-line"></div>
              <span class="separator-text">ou</span>
              <div class="separator-line"></div>
            </div>

            <!-- Instrução para URL -->
            <p class="url-instruction">Insira uma URL de imagem</p>
          </div>

          <!-- URL da Imagem -->
          <mat-form-field appearance="outline" class="form-field">
            <mat-label>URL da Imagem</mat-label>
            <input matInput formControlName="imageUrl" placeholder="https://exemplo.com/imagem.jpg">
            @if (imageForm.get('imageUrl')?.hasError('required') && imageForm.get('imageUrl')?.touched) {
              <mat-error>URL da imagem é obrigatória</mat-error>
            }
            @if (imageForm.get('imageUrl')?.hasError('pattern') && imageForm.get('imageUrl')?.touched) {
              <mat-error>URL deve ser válida</mat-error>
            }
          </mat-form-field>

          <!-- Preview da Imagem -->
          @if (imageForm.get('imageUrl')?.value && !imageForm.get('imageUrl')?.hasError('pattern')) {
            <div class="preview-container">
              <div class="preview-wrapper">
                <img 
                  [src]="imageForm.get('imageUrl')?.value" 
                  alt="Preview"
                  class="preview-image"
                  (error)="onImageError()">
                @if (imageLoading()) {
                  <div class="loading-overlay">
                    <mat-spinner diameter="40"></mat-spinner>
                  </div>
                }
              </div>
            </div>
          }

          <!-- Texto Alternativo -->
          <mat-form-field appearance="outline" class="form-field">
            <mat-label>Texto Alternativo (Alt Text)</mat-label>
            <input matInput formControlName="altText" placeholder="Descrição da imagem para acessibilidade">
            @if (imageForm.get('altText')?.hasError('required') && imageForm.get('altText')?.touched) {
              <mat-error>Texto alternativo é obrigatório</mat-error>
            }
          </mat-form-field>

          <!-- Legenda -->
          <mat-form-field appearance="outline" class="form-field">
            <mat-label>Legenda</mat-label>
            <input matInput formControlName="caption" placeholder="Legenda da imagem">
            @if (imageForm.get('caption')?.hasError('required') && imageForm.get('caption')?.touched) {
              <mat-error>Legenda é obrigatória</mat-error>
            }
          </mat-form-field>

          <!-- Ordem -->
          <mat-form-field appearance="outline" class="form-field">
            <mat-label>Ordem de Exibição</mat-label>
            <input matInput type="number" formControlName="order" placeholder="1, 2, 3...">
            @if (imageForm.get('order')?.hasError('required') && imageForm.get('order')?.touched) {
              <mat-error>Ordem é obrigatória</mat-error>
            }
            @if (imageForm.get('order')?.hasError('min') && imageForm.get('order')?.touched) {
              <mat-error>Ordem deve ser maior que 0</mat-error>
            }
          </mat-form-field>

          <!-- Checkbox para Imagem Principal -->
          <mat-checkbox formControlName="isMainImage" class="main-image-checkbox">
            Definir como imagem principal
          </mat-checkbox>
        </form>
      </div>

      <!-- Actions -->
      <div class="modal-actions">
        <button 
          type="button"
          mat-button 
          (click)="close()"
          class="cancel-btn">
          Cancelar
        </button>
        <button 
          type="submit"
          mat-raised-button 
          color="primary"
          [disabled]="imageForm.invalid || submitting()"
          (click)="onSubmit()"
          class="submit-btn">
          @if (submitting()) {
            <mat-spinner diameter="20"></mat-spinner>
          } @else {
            <mat-icon>{{ isEditMode() ? 'save' : 'add' }}</mat-icon>
          }
          {{ isEditMode() ? 'Salvar' : 'Adicionar' }}
        </button>
      </div>
    </div>
  `,
   styles: [`
    .image-upload-modal {
      width: 100%;
      max-width: 600px;
      margin: 0 auto;
    }

    .modal-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 1.5rem 1.5rem 0;
      border-bottom: 1px solid #e5e7eb;
      margin-bottom: 1.5rem;
    }

    .modal-title {
      font-size: 1.25rem;
      font-weight: 600;
      color: #334a52;
      margin: 0;
    }

    .close-button {
      color: #6b7280;
    }

    .close-button:hover {
      color: #374151;
    }

    .modal-content {
      padding: 0 1.5rem;
      max-height: 70vh;
      overflow-y: auto;
    }

    .form-container {
      display: flex;
      flex-direction: column;
      gap: 1rem;
    }

    .upload-area {
      border: 2px dashed #d1d5db;
      border-radius: 8px;
      padding: 1.5rem;
      text-align: center;
    }

    .upload-icon {
      font-size: 3rem;
      width: 3rem;
      height: 3rem;
      color: #9ca3af;
      margin-bottom: 0.5rem;
    }

    .upload-text {
      color: #6b7280;
      margin-bottom: 1rem;
    }

    .select-file-btn {
      background-color: #96afb8;
      color: white;
    }

    .select-file-btn:hover {
      background-color: #a288a9;
    }

    .file-info {
      margin-top: 1rem;
      padding: 0.75rem;
      background-color: #f9fafb;
      border-radius: 8px;
    }

    .file-name {
      font-size: 0.875rem;
      color: #374151;
      margin: 0 0 0.25rem 0;
    }

    .file-size {
      font-size: 0.75rem;
      color: #6b7280;
      margin: 0 0 0.5rem 0;
    }

    .upload-btn {
      background-color: #276678;
      color: white;
    }

    .upload-btn:hover {
      background-color: #334a52;
    }

    .upload-progress {
      margin-top: 1rem;
    }

    .progress-bar {
      margin-bottom: 0.5rem;
    }

    .progress-text {
      font-size: 0.875rem;
      color: #6b7280;
      margin: 0;
    }

    .separator {
      display: flex;
      align-items: center;
      margin: 1rem 0;
    }

    .separator-line {
      flex: 1;
      border-top: 1px solid #d1d5db;
    }

    .separator-text {
      padding: 0 0.75rem;
      color: #6b7280;
      font-size: 0.875rem;
    }

    .url-instruction {
      color: #6b7280;
      font-size: 0.875rem;
    }

    .form-field {
      width: 100%;
    }

    .preview-container {
      display: flex;
      justify-content: center;
    }

    .preview-wrapper {
      position: relative;
    }

    .preview-image {
      width: 200px;
      height: 150px;
      object-fit: cover;
      border-radius: 8px;
      border: 2px solid #d1d5db;
    }

    .loading-overlay {
      position: absolute;
      inset: 0;
      display: flex;
      align-items: center;
      justify-content: center;
      background-color: rgba(243, 244, 246, 0.75);
      border-radius: 8px;
    }

    .main-image-checkbox {
      color: #334a52;
      margin-top: 0.5rem;
    }

    .modal-actions {
      display: flex;
      justify-content: flex-end;
      gap: 1rem;
      padding: 1.5rem;
      border-top: 1px solid #e5e7eb;
      margin-top: 1.5rem;
    }

    .cancel-btn {
      color: #6b7280;
    }

    .cancel-btn:hover {
      color: #374151;
    }

    .submit-btn {
      background-color: #276678;
      color: white;
    }

    .submit-btn:hover {
      background-color: #334a52;
    }

    .mat-mdc-form-field {
      width: 100%;
    }

    .mat-mdc-checkbox {
      margin-top: 8px;
    }

    @media (max-width: 640px) {
      .image-upload-modal {
        max-width: 100%;
        margin: 0;
      }
      
      .modal-header,
      .modal-content,
      .modal-actions {
        padding-left: 1rem;
        padding-right: 1rem;
      }

      .preview-image {
        width: 180px;
        height: 135px;
      }
    }
  `]
})
export class ImageUploadModalComponent {
  private readonly fb = inject(FormBuilder);
  private readonly dialogRef = inject(MatDialogRef<ImageUploadModalComponent>);
  private readonly data = inject<ImageUploadData>(MAT_DIALOG_DATA);
  private readonly snackBar = inject(MatSnackBar);
  private readonly modalService = inject(ModalService);
  private readonly errorMapping = inject(ErrorMappingService);
  // Signals
  submitting = signal(false);
  imageLoading = signal(false);
  selectedFile = signal<File | null>(null);
  uploading = signal(false);
  uploadProgress = signal(0);

  // Computed
  isEditMode = computed(() => !!this.data.imageUrl);

  // Form
  imageForm: FormGroup = this.fb.group({
    imageUrl: [this.data.imageUrl || '', [
      Validators.required, 
      Validators.pattern(/^https?:\/\/.+/)
    ]],
    altText: [this.data.altText || '', [Validators.required, Validators.maxLength(200)]],
    caption: [this.data.caption || '', [Validators.required, Validators.maxLength(200)]],
    order: [this.data.order || 1, [Validators.required, Validators.min(1)]],
    isMainImage: [this.data.isMainImage || false]
  });

  constructor() {
    // Watch for image URL changes to show loading
    this.imageForm.get('imageUrl')?.valueChanges
      .pipe(takeUntilDestroyed())
      .subscribe(url => {
        if (url && this.imageForm.get('imageUrl')?.valid) {
          this.imageLoading.set(true);
          // Simulate image loading
          setTimeout(() => {
            this.imageLoading.set(false);
          }, 1000);
        }
      });
  }

  onFileSelected(event: any): void {
    const file = event.target.files[0];
    if (file) {
      // Validar tipo de arquivo
      const allowedTypes = ['image/jpeg', 'image/jpg', 'image/png', 'image/gif', 'image/webp'];
      if (!allowedTypes.includes(file.type)) {
        this.snackBar.open('Tipo de arquivo não permitido. Use apenas: jpg, jpeg, png, gif, webp', 'Fechar', {
          duration: 3000,
          panelClass: ['error-snackbar']
        });
        return;
      }

      // Validar tamanho (5MB)
      if (file.size > 5 * 1024 * 1024) {
        this.snackBar.open('Arquivo muito grande. Tamanho máximo: 5MB', 'Fechar', {
          duration: 3000,
          panelClass: ['error-snackbar']
        });
        return;
      }

      this.selectedFile.set(file);
      
      // Criar preview
      const reader = new FileReader();
      reader.onload = (e: any) => {
        this.imageForm.patchValue({ imageUrl: e.target.result });
      };
      reader.readAsDataURL(file);
    }
  }

  uploadFile(): void {
    if (!this.selectedFile()) return;

    this.uploading.set(true);
    this.uploadProgress.set(0);

    this.modalService.uploadImage(this.selectedFile()!).subscribe({
      next: (progress: UploadProgress) => {
        this.uploadProgress.set(progress.progress);
        
        if (progress.progress === 100 && progress.imageUrl) {
          this.imageForm.patchValue({ imageUrl: progress.imageUrl });
          this.snackBar.open('Upload realizado com sucesso!', 'Fechar', { duration: 3000 });
          this.uploading.set(false);
          this.uploadProgress.set(0);
        }
      },
      error: (error: Error) => {
        console.error('Erro no upload:', error);
        // Agora usa o error mapping
        const errorMessage = this.errorMapping.mapServiceOrderError(error);
        this.snackBar.open(errorMessage, 'Fechar', {
          duration: 3000,
          panelClass: ['error-snackbar']
        });
        this.uploading.set(false);
        this.uploadProgress.set(0);
      }
    });
  }

  onSubmit(): void {
    if (this.imageForm.invalid) {
      this.markFormGroupTouched();
      return;
    }

    this.submitting.set(true);

    // Simulate API call
    setTimeout(() => {
      const imageData: CreateWebsiteCaseImageDto = {
        imageUrl: this.imageForm.value.imageUrl,
        altText: this.imageForm.value.altText,
        caption: this.imageForm.value.caption,
        order: this.imageForm.value.order,
        isMainImage: this.imageForm.value.isMainImage
      };

      this.dialogRef.close(imageData);
      this.submitting.set(false);
    }, 1000);
  }

  onImageError(): void {
    Swal.fire({
      icon: 'error',
      title: 'Erro!',
      text: 'Erro ao carregar imagem. Verifique se a URL está correta.',
      confirmButtonColor: '#276678'
    });
  }

  close(): void {
    this.dialogRef.close();
  }

  private markFormGroupTouched(): void {
    Object.keys(this.imageForm.controls).forEach(key => {
      const control = this.imageForm.get(key);
      control?.markAsTouched();
    });
  }
}