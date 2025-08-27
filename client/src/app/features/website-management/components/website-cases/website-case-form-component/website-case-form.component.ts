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
import { MatDialogModule } from '@angular/material/dialog';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

import { WebsiteCaseService } from '../../../services/website-case.service';
import { 
  CreateWebsiteCaseDto, 
  UpdateWebsiteCaseDto,
  WebsiteCaseImage,
} from '../../../models/website-case.interface';
import { ErrorMappingService } from '../../../../../core/services/error.mapping.service';
import { WebsiteCaseImageManagerComponent } from '../website-case-image-manager-component/website-case-image-manager.component';

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
    WebsiteCaseImageManagerComponent,
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
  private readonly destroyRef = inject(DestroyRef);
  private readonly errorMapping = inject(ErrorMappingService);
  

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
      next: () => {
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

 

  onImageAdded(image: WebsiteCaseImage): void {
  this.additionalImages.update(images => [...images, image]);
}

onImageUpdated(event: { id: number, image: Partial<WebsiteCaseImage> }): void {
  this.additionalImages.update(images => 
    images.map(img => img.id === event.id ? { ...img, ...event.image } : img)
  );
}

onImageDeleted(imageId: number): void {
  this.additionalImages.update(images => images.filter(img => img.id !== imageId));
}

onImageReordered(event: { fromIndex: number, toIndex: number }): void {
  this.additionalImages.update(images => {
    const reordered = [...images];
    const [movedImage] = reordered.splice(event.fromIndex, 1);
    reordered.splice(event.toIndex, 0, movedImage);
    
    // Atualizar a ordem
    reordered.forEach((image, index) => {
      image.order = index + 1;
      image.isMainImage = index === 0;
    });
    
    return reordered;
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