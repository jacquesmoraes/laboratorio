import { Component, OnInit, inject, signal, ChangeDetectionStrategy, DestroyRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { Scale } from '../../../models/scale.interface';
import { ScaleService } from '../../../services/scale.service';
import { ScaleModalComponent, ScaleModalData } from '../scale-modal.component/scale-modal.component';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

import Swal from 'sweetalert2';

@Component({
  selector: 'app-scale-list',
  standalone: true,
  imports: [
    CommonModule,
    MatIconModule,
    MatDialogModule
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './scale-list.component.html',
  styleUrls: ['./scale-list.component.scss']
})
export class ScaleListComponent implements OnInit {
  private readonly scaleService = inject(ScaleService);
  private readonly dialog = inject(MatDialog);
 
  private readonly destroyRef = inject(DestroyRef);

  scales = signal<Scale[]>([]);

  ngOnInit(): void {
    this.loadScales();
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

  private openModal(data: ScaleModalData): void {
    this.dialog.open(ScaleModalComponent, {
      data,
      width: '500px'
    }).afterClosed()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((result) => {
        if (result) this.loadScales();
      });
  }

  onNew(): void {
    this.openModal({ isEditMode: false });
  }

  onEdit(scale: Scale): void {
    this.openModal({ scale, isEditMode: true });
  }

  deleteScale(id: number): void {
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
        this.scaleService.delete(id)
          .pipe(takeUntilDestroyed(this.destroyRef))
          .subscribe({
            next: () => {
              Swal.fire('Sucesso!', 'Escala excluída com sucesso', 'success');
              this.loadScales();
            },
            error: () => {
             
            }
          });
      }
    });
  }
}