import { ChangeDetectionStrategy, Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

import { Scale } from '../../../models/scale.interface';
import { Shade, UpdateShadeDto, CreateShadeDto } from '../../../models/shade.interface';
import { ScaleService } from '../../../services/scale.service';
import { ShadeService } from '../../../services/shade.service';

import { tap } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import Swal from 'sweetalert2';

export interface ShadeModalData {
  shade?: Shade;
  isEditMode: boolean;
}

@Component({
  selector: 'app-shade-modal',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatSelectModule,
    MatDialogModule
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './shade-modal.component.html',
  styleUrls: ['./shade-modal.component.scss']
})
export class ShadeModalComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly shadeService = inject(ShadeService);
  private readonly scaleService = inject(ScaleService);
  
  private readonly dialogRef = inject(MatDialogRef<ShadeModalComponent>);
  private readonly data = inject<ShadeModalData>(MAT_DIALOG_DATA);
  private readonly destroyRef = inject(DestroyRef);
  protected readonly shadeForm = this.fb.group({
    color: this.fb.control('', { nonNullable: true, validators: [Validators.required, Validators.minLength(2)] }),
    scaleId: this.fb.control<number | undefined>(undefined, { nonNullable: true, validators: [Validators.required] })
  });

  protected readonly isEditMode = signal(this.data.isEditMode);
  protected readonly scales = signal<Scale[]>([]);
  
  private shadeId?: number;

  ngOnInit(): void {
    this.loadScales()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => this.patchFormIfEdit(),
        error: () => {
         
        }
      });
  }

 private loadScales() {
  return this.scaleService.getAll().pipe(
    tap((scales: Scale[]) => this.scales.set(scales))
  );
}


  private patchFormIfEdit(): void {
    if (this.isEditMode() && this.data.shade) {
      this.shadeId = this.data.shade.id;
      this.shadeForm.patchValue({
        color: this.data.shade.color,
        scaleId: this.data.shade.scaleId
      });
    }
  }

  protected onSubmit(): void {
    if (this.shadeForm.invalid) return;

    const dto: CreateShadeDto | UpdateShadeDto = {
      color: this.shadeForm.value.color!,
      scaleId: this.shadeForm.value.scaleId!
    };

    const request$ = this.isEditMode() && this.shadeId
      ? this.shadeService.update(this.shadeId, dto)
      : this.shadeService.create(dto);

    request$.pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
         
          Swal.fire(
            'Sucesso!',
            this.isEditMode() ? 'Cor atualizada com sucesso' : 'Cor criada com sucesso',
            'success'
          );
          this.dialogRef.close(true);
        },
        error: () => {
         
        }
      });
  }

  protected onCancel(): void {
    this.dialogRef.close();
  }
}
