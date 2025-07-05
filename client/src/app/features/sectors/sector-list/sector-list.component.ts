import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import Swal from 'sweetalert2';

import { Sector } from '../models/sector.interface';
import { SectorService } from '../service/sector.service';
import { SectorModalComponent, SectorModalData } from '../sector-modal/sector-modal.component';
import { MatCardModule } from '@angular/material/card';

@Component({
  selector: 'app-sector-list',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule,
    MatDialogModule],
  templateUrl: './sector-list.component.html',
  styleUrls: ['./sector-list.component.scss'],
})
export class SectorListComponent implements OnInit {
  private sectorService = inject(SectorService);
  private dialog = inject(MatDialog);

  protected sectors = signal<Sector[]>([]);
  protected displayedColumns = ['name', 'actions'];

  ngOnInit(): void {
    this.loadSectors();
  }

  private loadSectors(): void {
    this.sectorService.getAll().subscribe({
      next: (sectors) => {
        this.sectors.set(sectors);
      },
      error: (error) => {
        console.error('Erro ao carregar setores:', error);
        Swal.fire({
          icon: 'error',
          title: 'Erro!',
          text: 'Erro ao carregar setores',
          confirmButtonText: 'OK'
        });
      }
    });
  }

  protected onNew(): void {
    const dialogRef = this.dialog.open(SectorModalComponent, {
      data: { isEditMode: false } as SectorModalData,
      width: '500px'
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.loadSectors();
      }
    });
  }

  protected onEdit(sector: Sector): void {
    const dialogRef = this.dialog.open(SectorModalComponent, {
      data: { sector, isEditMode: true } as SectorModalData,
      width: '500px'
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.loadSectors();
      }
    });
  }

  protected onDelete(sector: Sector): void {
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
        this.sectorService.delete(sector.id).subscribe({
          next: () => {
            Swal.fire({
              icon: 'success',
              title: 'Sucesso!',
              text: 'Setor excluído com sucesso',
              timer: 1000,
              showConfirmButton: false
            });
            this.loadSectors();
          },
          error: (error) => {
            console.error('Erro ao excluir setor:', error);
            Swal.fire({
              icon: 'error',
              title: 'Erro!',
              text: 'Erro ao excluir setor',
              confirmButtonText: 'OK'
            });
          }
        });
      }
    });
  }
}