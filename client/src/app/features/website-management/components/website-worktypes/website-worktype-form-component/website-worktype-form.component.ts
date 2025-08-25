import { Component, OnInit, signal, computed, inject, ChangeDetectionStrategy, DestroyRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSelectModule } from '@angular/material/select';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { WebsiteWorkTypeService } from '../../../services/website-worktype.service';
import { CreateWebsiteWorkTypeDto, UpdateWebsiteWorkTypeDto } from '../../../models/website-worktype.interface';
import { ModalService } from '../../shared/services/modal.service';
import { LoadingService } from '../../../../../core/services/loading.service';
import { WorkType } from '../../../../works/models/work-type.interface';
import { WorkTypeService } from '../../../../works/services/work-type.service';
import Swal, { SweetAlertResult } from 'sweetalert2';

@Component({
  selector: 'app-website-worktype-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatSelectModule,
    MatCheckboxModule
  ],
  templateUrl: './website-worktype-form.component.html',
  styleUrls: ['./website-worktype-form.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class WebsiteWorkTypeFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly workTypeService = inject(WebsiteWorkTypeService);
  private readonly workTypeBasicService = inject(WorkTypeService);
  private readonly modalService = inject(ModalService);
  private readonly snackBar = inject(MatSnackBar);
  private readonly destroyRef = inject(DestroyRef);
  private readonly loadingService = inject(LoadingService);

  // Signals
  isSubmitting = signal(false);
  workTypeId = signal<number | null>(null);
  availableWorkTypes = signal<WorkType[]>([]);

  // Computed
  isEditMode = computed(() => this.workTypeId() !== null);

  // Form
  workTypeForm: FormGroup = this.fb.group({
    workTypeId: [0, [Validators.required]],  
    imageUrl: ['', [Validators.required, Validators.pattern(/^https?:\/\/.+/)]],
    isActive: [true],
    order: [1, [Validators.required, Validators.min(1)]]
  });

  ngOnInit(): void {
    this.route.params
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(params => {
        const id = params['id'];
        if (id) {
          this.workTypeId.set(+id);
          this.loadWorkType(+id);
        }
      });
    
    this.loadAvailableWorkTypes();
  }

  loadWorkType(id: number): void {
    this.loadingService.show();
    
    this.workTypeService.getById(id).subscribe({
      next: (workType) => {
        this.workTypeForm.patchValue({
          workTypeId: workType.workTypeId,
          imageUrl: workType.imageUrl,
          isActive: workType.isActive,
          order: workType.order
        });
        this.loadingService.hide();
      },
      error: (error) => {
        console.error('Erro ao carregar work type:', error);
        this.snackBar.open('Erro ao carregar dados do serviço', 'Fechar', {
          duration: 3000,
          panelClass: ['error-snackbar']
        });
        this.loadingService.hide();
        this.router.navigate(['/admin/website-management/work-types']);
      }
    });
  }

  uploadImage(): void {
    this.modalService.openImageUploadModal().subscribe(result => {
      if (result) {
        this.workTypeForm.patchValue({
          imageUrl: result.imageUrl
        });
      }
    });
  }

  clearImage(): void {
    this.workTypeForm.patchValue({ imageUrl: '' });
  }

  onSubmit(): void {
    if (this.workTypeForm.invalid) {
      this.markFormGroupTouched();
      return;
    }
  
    this.isSubmitting.set(true);
    const formValue = this.workTypeForm.value;
  
    if (this.isEditMode()) {
      const updateData: UpdateWebsiteWorkTypeDto = {
        workTypeId: formValue.workTypeId,
        imageUrl: formValue.imageUrl,
        isActive: formValue.isActive,
        order: formValue.order
      };
  
      this.workTypeService.update(this.workTypeId()!, updateData).subscribe({
        next: () => {
          this.snackBar.open('Serviço atualizado com sucesso!', 'Fechar', { duration: 3000 });
          this.router.navigate(['/admin/website-management/work-types']);
        },
        error: (error) => {
          console.error('Erro ao atualizar:', error);
          this.snackBar.open('Erro ao atualizar serviço', 'Fechar', {
            duration: 3000,
            panelClass: ['error-snackbar']
          });
          this.isSubmitting.set(false);
        }
      });
    } else {
      const createData: CreateWebsiteWorkTypeDto = {
        workTypeId: formValue.workTypeId,
        imageUrl: formValue.imageUrl,
        isActive: formValue.isActive,
        order: formValue.order
      };
  
      this.workTypeService.create(createData).subscribe({
        next: () => {
          this.snackBar.open('Serviço criado com sucesso!', 'Fechar', { duration: 3000 });
          this.router.navigate(['/admin/website-management/work-types']);
        },
        error: (error) => {
          console.error('Erro ao criar:', error);
          this.snackBar.open('Erro ao criar serviço', 'Fechar', {
            duration: 3000,
            panelClass: ['error-snackbar']
          });
          this.isSubmitting.set(false);
        }
      });
    }
  }

  deleteWorkType(): void {
    if (!this.isEditMode() || !this.workTypeId()) return;

    Swal.fire({
      title: 'Confirmar exclusão',
      text: 'Tem certeza que deseja excluir este serviço? Esta ação não pode ser desfeita.',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#d33',
      cancelButtonColor: '#3085d6',
      confirmButtonText: 'Sim, excluir',
      cancelButtonText: 'Cancelar'
    }).then((result: SweetAlertResult) => {
      if (result.isConfirmed) {
        this.workTypeService.delete(this.workTypeId()!).subscribe({
          next: () => {
            this.snackBar.open('Serviço excluído com sucesso!', 'Fechar', { duration: 3000 });
            this.router.navigate(['/admin/website-management/work-types']);
          },
          error: (error) => {
            console.error('Erro ao excluir:', error);
            this.snackBar.open('Erro ao excluir serviço', 'Fechar', {
              duration: 3000,
              panelClass: ['error-snackbar']
            });
          }
        });
      }
    });
  }

  cancel(): void {
    Swal.fire({
      title: 'Confirmar saída',
      text: 'Tem certeza que deseja sair? As alterações não salvas serão perdidas.',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#d33',
      cancelButtonColor: '#3085d6',
      confirmButtonText: 'Sim, sair',
      cancelButtonText: 'Cancelar'
    }).then((result: SweetAlertResult) => {
      if (result.isConfirmed) {
        this.router.navigate(['/admin/website-management/work-types']);
      }
    });
  }

  onImageError(event: Event): void {
    const target = event.target as HTMLImageElement;
    if (target) {
      target.style.display = 'none';
    }
  }

  private loadAvailableWorkTypes(): void {
    this.workTypeBasicService.getAll().subscribe({
      next: (workTypes) => {
        this.availableWorkTypes.set(workTypes.filter(wt => wt.isActive));
      },
      error: (error) => {
        console.error('Erro ao carregar work types:', error);
        this.snackBar.open('Erro ao carregar tipos de trabalho', 'Fechar', {
          duration: 3000,
          panelClass: ['error-snackbar']
        });
      }
    });
  }

  private markFormGroupTouched(): void {
    Object.keys(this.workTypeForm.controls).forEach(key => {
      const control = this.workTypeForm.get(key);
      control?.markAsTouched();
    });
  }
}