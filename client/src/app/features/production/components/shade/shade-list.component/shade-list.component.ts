import { Component, OnInit, inject, signal, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { ReactiveFormsModule, FormBuilder } from '@angular/forms';

import { Shade } from '../../../models/shade.interface';
import { Scale } from '../../../models/scale.interface';
import { ShadeService } from '../../../services/shade.service';
import { ScaleService } from '../../../services/scale.service';
import { ShadeModalComponent, ShadeModalData } from '../shade-modal.component/shade-modal.component';
import { ErrorService } from '../../../../../core/services/error.service';

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
  styleUrls: ['./shade-list.component.scss']
})
export class ShadeListComponent implements OnInit {
  private readonly shadeService = inject(ShadeService);
  private readonly scaleService = inject(ScaleService);
  private readonly dialog = inject(MatDialog);
  private readonly fb = inject(FormBuilder);
  private readonly errorService = inject(ErrorService);

  protected readonly shades = signal<Shade[]>([]);
  protected readonly scales = signal<Scale[]>([]);
  protected readonly displayedColumns = [ 'scaleId', 'color', 'actions'];
  
  protected readonly filterForm = this.fb.group({
    scaleId: [undefined]
  });
  trackByShadeId = (_: number, shade: { id: number }) => shade.id;
trackByScaleId = (_: number, scale: { id: number }) => scale.id;


  ngOnInit(): void {
    this.loadScales();
    this.loadShades();
  }

  private loadScales(): void {
    this.scaleService.getAll().subscribe({
      next: (data) => this.scales.set(data),
      error: (error) => this.errorService.showError('Erro ao carregar escalas', error)
    });
  }

  private loadShades(scaleId?: number): void {
    this.shadeService.getAll(scaleId).subscribe({
      next: (data) => this.shades.set(data),
      error: (error) => this.errorService.showError('Erro ao carregar cores', error)
    });
  }

  protected onScaleFilterChange(): void {
    const scaleId = this.filterForm.get('scaleId')?.value ?? undefined;
    this.loadShades(scaleId);
  }

  protected onNew(): void {
    this.dialog.open(ShadeModalComponent, {
      data: { isEditMode: false } as ShadeModalData,
      width: '550px'
    }).afterClosed().subscribe((result) => {
      if (result) {
        const scaleId = this.filterForm.get('scaleId')?.value ?? undefined;
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
        const scaleId = this.filterForm.get('scaleId')?.value ?? undefined;
        this.loadShades(scaleId);
      }
    });
  }

  protected deleteShade(id: number): void {
    this.errorService.confirm('Tem certeza?', 'Esta ação não pode ser desfeita!').then((result) => {
      if (result.isConfirmed) {
        this.shadeService.delete(id).subscribe({
          next: () => {
            this.errorService.showSuccess('Cor excluída com sucesso');
            const scaleId = this.filterForm.get('scaleId')?.value ?? undefined;
            this.loadShades(scaleId);
          },
          error: (error) => this.errorService.showError('Erro ao excluir cor', error)
        });
      }
    });
  }

  protected getScaleName(scaleId: number): string {
    const scale = this.scales().find(s => s.id === scaleId);
    return scale ? scale.name : 'N/A';
  }
}