import { Component, inject, signal, computed, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { FormBuilder, FormGroup, FormArray, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDividerModule } from '@angular/material/divider';
import { SweetAlert2Module } from '@sweetalert2/ngx-sweetalert2';
import { TablePriceService } from '../../services/table-price.services';
import { CreateTablePriceDto, UpdateTablePriceDto, WorkType } from '../../table-price.interface';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-table-price-form',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatCheckboxModule,
    MatProgressSpinnerModule,
    MatDividerModule,
    SweetAlert2Module
  ],
  templateUrl: './table-price-form.component.html',
  styleUrls: ['./table-price-form.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TablePriceFormComponent implements OnInit {
  private tablePriceService = inject(TablePriceService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private fb = inject(FormBuilder);

  form!: FormGroup;
  workTypes = signal<WorkType[]>([]);
  loading = signal(true);
  submitting = signal(false);
  isEditing = signal(false);
  tablePriceId = signal<number | null>(null);
  isNew = computed(() => !this.isEditing());

  get itemsArray(): FormArray {
    return this.form.get('items') as FormArray;
  }

  ngOnInit(): void {
    this.loadWorkTypes();
    this.initializeForm();
    this.checkIfEditing();
  }

  private loadWorkTypes(): void {
    this.tablePriceService.getWorkTypes().subscribe({
      next: (data) => {
        this.workTypes.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Error loading work types:', err);
        this.loading.set(false);
      }
    });
  }

  private initializeForm(): void {
    this.form = this.fb.group({
      name: ['', [Validators.required]],
      description: [''],
      status: [true],
      items: this.fb.array([])
    });
  }

  private checkIfEditing(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    const id = idParam ? Number(idParam) : null;
    if (id !== null && !isNaN(id)) {
      this.isEditing.set(true);
      this.tablePriceId.set(id);
      this.loadTablePrice(id);
    }
  }

  private loadTablePrice(id: number): void {
    this.loading.set(true);
    this.tablePriceService.getTablePriceById(id).subscribe({
      next: (data) => {
        this.form.patchValue({
          name: data.name,
          description: data.description,
          status: data.status
        });

        while (this.itemsArray.length !== 0) {
          this.itemsArray.removeAt(0);
        }

        data.items.forEach(item => {
          this.itemsArray.push(this.fb.group({
            workTypeId: [item.workTypeId, [Validators.required]],
            price: [item.price, [Validators.required, Validators.min(0)]]
          }));
        });

        this.loading.set(false);
      },
      error: (err) => {
        console.error('Error loading table price:', err);
        this.loading.set(false);
      }
    });
  }

  addItem(): void {
    const group: FormGroup = this.fb.group({
      workTypeId: ['', [Validators.required]],
      price: ['', [Validators.required, Validators.min(0)]]
    });
    this.itemsArray.push(group);
  }

  removeItem(index: number): void {
    Swal.fire({
      title: 'Remover item?',
      text: 'Tem certeza que deseja remover este item?',
      icon: 'question',
      showCancelButton: true,
      confirmButtonText: 'Sim, remover!',
      cancelButtonText: 'Cancelar'
    }).then((result) => {
      if (result.isConfirmed) {
        this.itemsArray.removeAt(index);
        Swal.fire('Removido!', 'Item removido com sucesso.', 'success');
      }
    });
  }

  onSubmit(): void {
    if (this.form.invalid) return;

    this.submitting.set(true);
    const formValue = this.form.value;

    const dto: CreateTablePriceDto = {
      name: formValue.name,
      description: formValue.description,
      items: formValue.items.map((item: any) => ({
        workTypeId: item.workTypeId,
        price: item.price
      }))
    };

    if (this.isEditing()) {
      const updateDto: UpdateTablePriceDto = {
        ...dto,
        id: this.tablePriceId()!,
        status: formValue.status
      };

      this.tablePriceService.updateTablePrice(this.tablePriceId()!, updateDto).subscribe({
        next: () => {
          Swal.fire('Atualizado!', 'Tabela de preço atualizada com sucesso.', 'success').then(() => {
            this.router.navigate(['/table-price']);
          });
        },
        error: (err) => {
          this.submitting.set(false);
          Swal.fire('Erro!', 'Erro ao atualizar tabela de preço.', 'error');
          console.error('Error updating table price:', err);
        }
      });
    } else {
      this.tablePriceService.createTablePrice(dto).subscribe({
        next: () => {
          Swal.fire('Criado!', 'Tabela de preço criada com sucesso.', 'success').then(() => {
            this.router.navigate(['/table-price']);
          });
        },
        error: (err) => {
          this.submitting.set(false);
          Swal.fire('Erro!', 'Erro ao criar tabela de preço.', 'error');
          console.error('Error creating table price:', err);
        }
      });
    }
  }

  goBack(): void {
    Swal.fire({
      title: 'Sair sem salvar?',
      text: 'Tem certeza que deseja sair? As alterações serão perdidas.',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: 'Sim, sair!',
      cancelButtonText: 'Cancelar'
    }).then((result) => {
      if (result.isConfirmed) {
        this.router.navigate(['/table-price']);
      }
    });
  }
}
