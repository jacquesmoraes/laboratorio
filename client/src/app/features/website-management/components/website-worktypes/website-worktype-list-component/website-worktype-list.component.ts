import { Component, OnInit, signal, inject, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatCardModule } from '@angular/material/card';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar } from '@angular/material/snack-bar';
import { WebsiteWorkTypeService } from '../../../services/website-worktype.service';
import { WebsiteWorkType } from '../../../models/website-worktype.interface';

import Swal from 'sweetalert2';
import { LoadingService } from '../../../../../core/services/loading.service';

@Component({
  selector: 'app-website-worktype-list',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatCardModule,
    MatTooltipModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './website-worktype-list.component.html',
  styleUrls: ['./website-worktype-list.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class WebsiteWorkTypeListComponent implements OnInit {
  private workTypeService = inject(WebsiteWorkTypeService);
  private router = inject(Router);
  private snackBar = inject(MatSnackBar);
  private loadingService = inject(LoadingService);

  readonly workTypes = signal<WebsiteWorkType[]>([]);
  readonly displayedColumns = ['image', 'name', 'description', 'isActive', 'order', 'actions'];

  ngOnInit(): void {
    this.loadWorkTypes();
  }

  private loadWorkTypes(): void {
    this.workTypeService.getAll().subscribe({
      next: (workTypes) => {
        this.workTypes.set(workTypes);
      },
      error: (error) => {
        console.error('Erro ao carregar work types:', error);
        this.snackBar.open('Erro ao carregar serviços', 'Fechar', {
          duration: 3000,
          panelClass: ['error-snackbar']
        });
      }
    });
  }

  createWorkType(): void {
    this.router.navigate(['/admin/website-management/work-types/new']);
  }

  editWorkType(workType: WebsiteWorkType): void {
    this.router.navigate(['/admin/website-management/work-types', workType.id, 'edit']);
  }

  viewWorkType(workType: WebsiteWorkType): void {
    this.router.navigate(['/admin/website-management/work-types', workType.id]);
  }

  toggleActive(workType: WebsiteWorkType): void {
    const action = workType.isActive ? 'desativar' : 'ativar';
    
    Swal.fire({
      title: `Confirmar ${action}`,
      text: `Tem certeza que deseja ${action} o serviço "${workType.workTypeName}"?`,
      icon: 'question',
      showCancelButton: true,
      confirmButtonColor: workType.isActive ? '#d33' : '#28a745',
      cancelButtonColor: '#6c757d',
      confirmButtonText: `Sim, ${action}`,
      cancelButtonText: 'Cancelar'
    }).then((result) => {
      if (result.isConfirmed) {
        this.workTypeService.toggleActive(workType.id).subscribe({
          next: () => {
            // Atualizar o estado local
            this.workTypes.update(types => 
              types.map(wt => 
                wt.id === workType.id 
                  ? { ...wt, isActive: !wt.isActive }
                  : wt
              )
            );
            
            this.snackBar.open(
              `Serviço ${workType.isActive ? 'desativado' : 'ativado'} com sucesso!`, 
              'Fechar', 
              { duration: 3000 }
            );
          },
          error: (error) => {
            console.error('Erro ao alterar status:', error);
            this.snackBar.open('Erro ao alterar status do serviço', 'Fechar', {
              duration: 3000,
              panelClass: ['error-snackbar']
            });
          }
        });
      }
    });
  }

  deleteWorkType(workType: WebsiteWorkType): void {
    Swal.fire({
      title: 'Confirmar exclusão',
      text: `Tem certeza que deseja excluir o serviço "${workType.workTypeName}"? Esta ação não pode ser desfeita.`,
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#d33',
      cancelButtonColor: '#3085d6',
      confirmButtonText: 'Sim, excluir',
      cancelButtonText: 'Cancelar'
    }).then((result) => {
      if (result.isConfirmed) {
        this.workTypeService.delete(workType.id).subscribe({
          next: () => {
            // Remover da lista local
            this.workTypes.update(types => 
              types.filter(wt => wt.id !== workType.id)
            );
            
            this.snackBar.open('Serviço excluído com sucesso!', 'Fechar', { duration: 3000 });
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

  getStatusColor(isActive: boolean): string {
    return isActive ? 'accent' : 'warn';
  }

  getStatusText(isActive: boolean): string {
    return isActive ? 'Ativo' : 'Inativo';
  }

  onImageError(event: Event): void {
    const target = event.target as HTMLImageElement;
    if (target) {
      target.style.display = 'none';
    }
  }
}