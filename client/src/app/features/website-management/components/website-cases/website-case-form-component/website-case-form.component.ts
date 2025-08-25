import { Component, OnInit, signal, computed, inject, ChangeDetectionStrategy, DestroyRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatChipsModule } from '@angular/material/chips';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

import { WebsiteCaseService } from '../../../services/website-case.service';
import { 
  CreateWebsiteCaseDto, 
  UpdateWebsiteCaseDto,
  WebsiteCaseAdmin,
  WebsiteCaseImage,
} from '../../../models/website-case.interface';
    
import { ModalService } from '../../shared/services/modal.service';
import { ErrorMappingService } from '../../../../../core/services/error.mapping.service';
import { LoadingService } from '../../../../../core/services/loading.service';
import Swal, { SweetAlertResult } from 'sweetalert2';

@Component({
  selector: 'app-website-case-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatCheckboxModule,
    MatProgressSpinnerModule,
    MatTooltipModule,
    MatChipsModule,
    MatDialogModule
  ],
  templateUrl: './website-case-form.component.html',
  styleUrls: ['./website-case-form.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class WebsiteCaseFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly websiteCaseService = inject(WebsiteCaseService);
  private readonly snackBar = inject(MatSnackBar);
  private readonly dialog = inject(MatDialog);
  private readonly destroyRef = inject(DestroyRef);
  private readonly modalService = inject(ModalService);
  private readonly errorMapping = inject(ErrorMappingService);
  private readonly loadingService = inject(LoadingService);

  // Signals
  submitting = signal(false);
  caseId = signal<number | null>(null);
  additionalImages = signal<WebsiteCaseImage[]>([]);

  // Computed
  isEditMode = computed(() => this.caseId() !== null);

  // Form
  caseForm: FormGroup = this.fb.group({
    title: ['', [Validators.required, Validators.maxLength(200)]],
    shortDescription: ['', [Validators.required, Validators.maxLength(500)]],
    fullDescription: ['', [Validators.required, Validators.maxLength(2000)]],
    mainImageUrl: ['', [Validators.required]],
    materials: ['', [Validators.required, Validators.maxLength(1000)]],
    procedure: ['', [Validators.required, Validators.maxLength(2000)]],
    results: ['', [Validators.required, Validators.maxLength(1000)]],
    patientInfo: ['', [Validators.required, Validators.maxLength(500)]],
    isActive: [true],
    order: [1, [Validators.required, Validators.min(1)]]
  });

  ngOnInit(): void {
    this.route.params
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(params => {
        const id = params['id'];
        if (id) {
          this.caseId.set(+id);
          this.loadCase(+id);
        }
      });
  }

  loadCase(id: number): void {
    this.websiteCaseService.getById(id).subscribe({
      next: (caseData) => {
        this.caseForm.patchValue({
          title: caseData.title,
          shortDescription: caseData.shortDescription,
          fullDescription: caseData.fullDescription,
          mainImageUrl: caseData.mainImageUrl,
          materials: caseData.materials,
          procedure: caseData.procedure,
          results: caseData.results,
          patientInfo: caseData.patientInfo,
          isActive: caseData.isActive,
          order: caseData.order
        });
        this.additionalImages.set(caseData.images || []);
      },
      error: (error) => {
        console.error('Erro ao carregar caso:', error);
        const errorMessage = this.errorMapping.mapServiceOrderError(error);
        this.snackBar.open(errorMessage, 'Fechar', {
          duration: 3000,
          panelClass: ['error-snackbar']
        });
      }
    });
  }

  onSubmit(): void {
    if (this.caseForm.invalid) {
      this.markFormGroupTouched();
      return;
    }

    this.submitting.set(true);
    const formValue = this.caseForm.value;

    const caseData = {
      ...formValue,
      images: this.additionalImages().map(img => ({
        imageUrl: img.imageUrl,
        altText: img.altText,
        caption: img.caption,
        order: img.order,
        isMainImage: img.isMainImage
      }))
    };

    const request = this.isEditMode() 
      ? this.websiteCaseService.update(this.caseId()!, caseData as UpdateWebsiteCaseDto)
      : this.websiteCaseService.create(caseData as CreateWebsiteCaseDto);

    request.subscribe({
      next: (result) => {
        this.snackBar.open(
          `Caso ${this.isEditMode() ? 'atualizado' : 'criado'} com sucesso!`, 
          'Fechar', 
          { duration: 3000 }
        );
        this.router.navigate(['/admin/website-management/cases']);
      },
      error: (error) => {
        console.error('Erro ao salvar caso:', error);
        const errorMessage = this.errorMapping.mapServiceOrderError(error);
        this.snackBar.open(errorMessage, 'Fechar', { 
          duration: 3000, 
          panelClass: ['error-snackbar'] 
        });
        this.submitting.set(false);
      }
    });
  }

  addImage(): void {
    this.modalService.openImageUploadModal().subscribe(result => {
      if (result) {
        const newImage: WebsiteCaseImage = {
          id: Date.now(), // Temporary ID
          imageUrl: result.imageUrl,
          altText: result.altText,
          caption: result.caption,
          order: result.order,
          isMainImage: result.isMainImage,
          createdAt: new Date().toISOString()
        };

        // Se esta imagem for marcada como principal, desmarcar as outras
        if (result.isMainImage) {
          this.additionalImages.update(images => 
            images.map(img => ({ ...img, isMainImage: false }))
          );
        }

        this.additionalImages.update(images => [...images, newImage]);
        this.snackBar.open('Imagem adicionada com sucesso!', 'Fechar', { duration: 3000 });
      }
    });
  }

  editImage(image: WebsiteCaseImage): void {
    const imageData = {
      imageUrl: image.imageUrl,
      altText: image.altText,
      caption: image.caption,
      order: image.order,
      isMainImage: image.isMainImage
    };

    this.modalService.openImageUploadModal(imageData).subscribe(result => {
      if (result) {
        const updatedImage: WebsiteCaseImage = {
          ...image,
          imageUrl: result.imageUrl,
          altText: result.altText,
          caption: result.caption,
          order: result.order,
          isMainImage: result.isMainImage
        };

        // Se esta imagem for marcada como principal, desmarcar as outras
        if (result.isMainImage) {
          this.additionalImages.update(images => 
            images.map(img => ({ 
              ...img, 
              isMainImage: img.id === image.id ? true : false 
            }))
          );
        } else {
          this.additionalImages.update(images => 
            images.map(img => img.id === image.id ? updatedImage : img)
          );
        }

        this.snackBar.open('Imagem atualizada com sucesso!', 'Fechar', { duration: 3000 });
      }
    });
  }

  removeImage(image: WebsiteCaseImage): void {
    Swal.fire({
      title: 'Confirmar remoção',
      text: `Tem certeza que deseja remover a imagem "${image.caption}"?`,
      icon: 'question',
      showCancelButton: true,
      confirmButtonColor: '#d33',
      cancelButtonColor: '#3085d6',
      confirmButtonText: 'Sim, remover!',
      cancelButtonText: 'Cancelar'
    }).then((result: SweetAlertResult) => {
      if (result.isConfirmed) {
        this.additionalImages.update(images => 
          images.filter(img => img.id !== image.id)
        );
        this.snackBar.open('Imagem removida com sucesso!', 'Fechar', { duration: 3000 });
      }
    });
  }

  clearMainImage(): void {
    this.caseForm.patchValue({ mainImageUrl: '' });
  }

  goBack(): void {
    this.router.navigate(['/admin/website-management/cases']);
  }

  private markFormGroupTouched(): void {
    Object.keys(this.caseForm.controls).forEach(key => {
      const control = this.caseForm.get(key);
      control?.markAsTouched();
    });
  }
}