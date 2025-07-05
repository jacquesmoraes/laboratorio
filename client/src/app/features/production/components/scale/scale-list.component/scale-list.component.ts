import { Component, OnInit, inject, signal, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { Scale } from '../../../models/scale.interface';
import { ScaleService } from '../../../services/scale.service';
import { ScaleModalComponent, ScaleModalData } from '../scale-modal.component/scale-modal.component';
import { ErrorService } from '../../../../../core/services/error.service';


@Component({
  selector: 'app-scale-list',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule,
    MatDialogModule
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './scale-list.component.html',
  styleUrls: ['./scale-list.component.scss']
})
export class ScaleListComponent implements OnInit {
  private readonly scaleService = inject(ScaleService);
  private readonly dialog = inject(MatDialog);
  private readonly errorService = inject(ErrorService);

  protected readonly scales = signal<Scale[]>([]);
  protected readonly displayedColumns: string[] = [ 'name', 'actions'];

  ngOnInit(): void {
    this.loadScales();
  }

  private loadScales(): void {
    this.scaleService.getAll().subscribe({
      next: (data) => this.scales.set(data),
      error: (error) => this.errorService.showError('Erro ao carregar escalas', error)
    });
  }

  protected onNew(): void {
    this.dialog.open(ScaleModalComponent, {
      data: { isEditMode: false } as ScaleModalData,
      width: '500px'
    }).afterClosed().subscribe((result) => {
      if (result) this.loadScales();
    });
  }

  protected onEdit(scale: Scale): void {
    this.dialog.open(ScaleModalComponent, {
      data: { scale, isEditMode: true } as ScaleModalData,
      width: '500px'
    }).afterClosed().subscribe((result) => {
      if (result) this.loadScales();
    });
  }

  protected deleteScale(id: number): void {
    this.errorService.confirm('Tem certeza?', 'Esta ação não pode ser desfeita!').then((result) => {
      if (result.isConfirmed) {
        this.scaleService.delete(id).subscribe({
          next: () => {
            this.errorService.showSuccess('Escala excluída com sucesso');
            this.loadScales();
          },
          error: (error) => this.errorService.showError('Erro ao excluir escala', error)
        });
      }
    });
  }
} 