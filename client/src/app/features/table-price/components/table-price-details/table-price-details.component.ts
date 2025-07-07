import { Component, inject, signal, computed, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTableModule } from '@angular/material/table';
import { MatDividerModule } from '@angular/material/divider';
import { SweetAlert2Module } from '@sweetalert2/ngx-sweetalert2';
import { TablePriceService } from '../../services/table-price.services';
import { TablePrice } from '../../table-price.interface';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-table-price-details',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatProgressSpinnerModule,
    MatTableModule,
    MatDividerModule,
    SweetAlert2Module
  ],
  templateUrl: './table-price-details.component.html',
  styleUrls: ['./table-price-details.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TablePriceDetailsComponent {
  private tablePriceService = inject(TablePriceService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  tablePrice = signal<TablePrice | null>(null);
  loading = signal(true);
  error = signal<string | null>(null);
 itemsColumns: string[] = ['workTypeName', 'price'];
  clientsColumns: string[] = ['clientName', 'tablePriceName'];
  hasItems = computed(() => (this.tablePrice()?.items.length ?? 0) > 0);
  hasClients = computed(() => (this.tablePrice()?.clients.length ?? 0) > 0);

  constructor() {
    const idParam = this.route.snapshot.paramMap.get('id');
    const id = idParam ? Number(idParam) : null;
    if (id !== null && !isNaN(id)) {
      this.loadTablePrice(id);
    }
  }

  private loadTablePrice(id: number): void {
    this.loading.set(true);
    this.error.set(null);

    this.tablePriceService.getTablePriceById(id).subscribe({
      next: (data) => {
        this.tablePrice.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set('Erro ao carregar tabela de preço');
        this.loading.set(false);
        console.error('Error loading table price:', err);
      }
    });
  }

  onEdit(): void {
    this.router.navigate(['/table-price', this.tablePrice()!.id, 'edit']);
  }

  deleteTablePrice(): void {
    if (!this.tablePrice()) return;

    Swal.fire({
      title: 'Tem certeza?',
      text: 'Esta ação não poderá ser revertida!',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: 'Sim, excluir!',
      cancelButtonText: 'Cancelar'
    }).then((result) => {
      if (result.isConfirmed) {
        this.tablePriceService.deleteTablePrice(this.tablePrice()!.id).subscribe({
          next: () => {
            Swal.fire('Excluído!', 'A tabela de preço foi excluída com sucesso.', 'success').then(() => {
              this.router.navigate(['/table-price']);
            });
          },
          error: (err) => {
            Swal.fire('Erro!', 'Erro ao excluir tabela de preço.', 'error');
            console.error('Error deleting table price:', err);
          }
        });
      }
    });
  }

 
  goBack(): void {
    this.router.navigate(['/table-price']);
  }
}
