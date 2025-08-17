import { Component, OnInit, inject, signal, ChangeDetectionStrategy, DestroyRef, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { ReactiveFormsModule, FormBuilder, FormGroup, FormControl } from '@angular/forms';

import { Shade } from '../../../models/shade.interface';
import { Scale } from '../../../models/scale.interface';
import { ShadeService } from '../../../services/shade.service';
import { ScaleService } from '../../../services/scale.service';
import { ShadeModalComponent, ShadeModalData } from '../shade-modal.component/shade-modal.component';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-shade-list',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule,
    MatSelectModule,
    MatFormFieldModule,
    MatDialogModule,
    ReactiveFormsModule
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './shade-list.component.html',
  
})
export class ShadeListComponent implements OnInit {
  private readonly shadeService = inject(ShadeService);
  private readonly scaleService = inject(ScaleService);
  private readonly dialog = inject(MatDialog);
  private readonly fb = inject(FormBuilder);
  
  private readonly destroyRef = inject(DestroyRef);
  protected readonly shades = signal<Shade[]>([]);
  protected readonly scales = signal<Scale[]>([]);
  protected readonly displayedColumns = ['scaleId', 'color', 'actions'];

  protected filterForm!: FormGroup<{
    scaleId: FormControl<number | null | undefined>;
  }>;


  trackByShadeId = (_: number, shade: Shade) => shade.id;
  trackByScaleId = (_: number, scale: Scale) => scale.id;

  ngOnInit(): void {
    this.filterForm = this.fb.group({
      scaleId: this.fb.control<number | null | undefined>(null)
    });

    this.loadScales();
    this.loadShades();
  }

  private loadScales(): void {
    this.scaleService.getAll()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (data) => this.scales.set(data),
        error: () => {
        
        }
      });
  }

  private loadShades(scaleId?: number): void {
    this.shadeService.getAll(scaleId)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (data) => this.shades.set(data),
        error: () => {
          
        }
      });
  }

  protected onScaleFilterChange(): void {
    const scaleId = this.filterForm.value.scaleId ?? undefined;
    this.loadShades(scaleId);
  }

  protected onNew(): void {
    this.dialog.open(ShadeModalComponent, {
      data: { isEditMode: false } as ShadeModalData,
      width: '550px'
    }).afterClosed().subscribe((result) => {
      if (result) {
        const scaleId = this.filterForm.value.scaleId ?? undefined;
        this.loadShades(scaleId);
      }
    });
  }

  protected onEdit(shade: Shade): void {
    this.dialog.open(ShadeModalComponent, {
      data: { shade, isEditMode: true } as ShadeModalData,
      width: '550px'
    }).afterClosed().subscribe((result) => {
      if (result) {
        const scaleId = this.filterForm.value.scaleId ?? undefined;
        this.loadShades(scaleId);
      }
    });
  }

  protected deleteShade(id: number): void {
    Swal.fire({
      title: 'Tem certeza?',
      text: 'Esta ação não pode ser desfeita!',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#d33',
      cancelButtonColor: '#3085d6',
      confirmButtonText: 'Sim, excluir!',
      cancelButtonText: 'Cancelar'
    }).then((result) => {
      if (result.isConfirmed) {
        this.shadeService.delete(id)
          .pipe(takeUntilDestroyed(this.destroyRef))
          .subscribe({
            next: () => {
              Swal.fire('Sucesso!', 'Cor excluída com sucesso', 'success');
              const scaleId = this.filterForm.value.scaleId ?? undefined;
              this.loadShades(scaleId);
            },
            error: () => {
             
            }
          });
      }
    });
  }

  protected readonly getScaleName = computed(() => {
    return (scaleId: number): string => {
      const scale = this.scales().find(s => s.id === scaleId);
      return scale ? scale.name : 'N/A';
    };
  });
}
