import { Component, OnInit, inject, ChangeDetectionStrategy, signal, DestroyRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import Swal from 'sweetalert2';

import { WorkSection } from '../../models/work-section.interface';
import { WorkSectionService } from '../../services/works-section.service';
import { WorkSectionModalComponent, WorkSectionModalData } from '../work-section-modal/work-section-modal.component';

import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-work-section-list',
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
  templateUrl: './work-section-list.component.html'
  
})
export class WorkSectionListComponent implements OnInit {
  private workSectionService = inject(WorkSectionService);
  private dialog = inject(MatDialog);
  private readonly destroyRef = inject(DestroyRef);
  workSections = signal<WorkSection[]>([]);
  displayedColumns: string[] = [ 'name', 'actions'];

  ngOnInit(): void {
    
    this.loadWorkSections();
  }

  loadWorkSections(): void {
    this.workSectionService.getAll()
      .pipe(takeUntilDestroyed(this.destroyRef)) 
      .subscribe({
        next: (data) => {
          this.workSections.set(data);
        },
        error: () => {
         
        }
      });
  }

  onNew(): void {
    const dialogRef = this.dialog.open(WorkSectionModalComponent, {
      data: { isEditMode: false } as WorkSectionModalData,
      width: '500px'
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.loadWorkSections();
      }
    });
  }

  onEdit(workSection: WorkSection): void {
    const dialogRef = this.dialog.open(WorkSectionModalComponent, {
      data: { workSection, isEditMode: true } as WorkSectionModalData,
      width: '500px'
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.loadWorkSections();
      }
    });
  }

  deleteWorkSection(id: number): void {
    console.log('Tentando deletar seção:', id); // ← Adicionar este log
    
    Swal.fire({
      title: 'Tem certeza?',
      text: 'Esta ação não pode ser desfeita!',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#d33',
      confirmButtonText: 'Sim, excluir!',
      cancelButtonText: 'Cancelar'
    }).then((result) => {
      if (result.isConfirmed) {
        console.log('Confirmação aceita, executando delete...'); // ← Adicionar este log
        this.performDelete(id);
      }
    });
  }
  
  private performDelete(id: number): void {
    console.log('Executando delete para ID:', id); // ← Adicionar este log
    
    this.workSectionService.delete(id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          console.log('Delete executado com sucesso'); // ← Adicionar este log
          Swal.fire({
            icon: 'success',
            title: 'Sucesso!',
            text: 'Seção de trabalho excluída com sucesso',
            timer: 2000,
            showConfirmButton: false
          });
          this.loadWorkSections();
        },
        error: (error) => {
          console.error('Erro no delete:', error); // ← Adicionar este log
        }
      });
  }
}