import { Component, inject, signal, computed, ChangeDetectionStrategy, DestroyRef, OnInit } from '@angular/core';
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
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

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
export class TablePriceDetailsComponent  implements OnInit {
  private tablePriceService = inject(TablePriceService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private readonly destroyRef = inject(DestroyRef);
  tablePrice = signal<TablePrice | null>(null);
  loading = signal(true);
  error = signal<string | null>(null);
 itemsColumns: string[] = ['workTypeName', 'price'];
  clientsColumns: string[] = ['clientName', 'tablePriceName'];
  hasItems = computed(() => (this.tablePrice()?.items.length ?? 0) > 0);
  hasClients = computed(() => (this.tablePrice()?.clients.length ?? 0) > 0);

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    const id = idParam ? Number(idParam) : null;
    if (id !== null && !isNaN(id)) {
      this.loadTablePrice(id);
    }
  }

  private loadTablePrice(id: number): void {
    this.loading.set(true);
    this.error.set(null);

    this.tablePriceService.getTablePriceById(id)
      .pipe(takeUntilDestroyed(this.destroyRef)) 
      .subscribe({
        next: (data) => {
          this.tablePrice.set(data);
          this.loading.set(false);
        },
        error: () => {
         
          this.loading.set(false);
         
        }
      });
  }

  onEdit(): void {
    this.router.navigate(['/admin/table-price', this.tablePrice()!.id, 'edit']);
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
        this.tablePriceService.deleteTablePrice(this.tablePrice()!.id)
          .pipe(takeUntilDestroyed(this.destroyRef))
          .subscribe({
            next: () => {
              Swal.fire('Excluído!', 'A tabela de preço foi excluída com sucesso.', 'success').then(() => {
                this.router.navigate(['/admin/table-price']);
              });
            },
            error: () => {
             
            }
          });
      }
    });
  }

 
  goBack(): void {
    this.router.navigate(['/admin/table-price']);
  }
}
