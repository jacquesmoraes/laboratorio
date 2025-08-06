import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { Sector } from '../../../../sectors/models/sector.interface';
import { MatNativeDateModule } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';

@Component({
  selector: 'app-move-to-stage-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule,
  ],
  template: `
    <h2 mat-dialog-title>Mover para Setor</h2>
    <mat-dialog-content>
      <form [formGroup]="form">
        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Setor</mat-label>
          <mat-select formControlName="sectorId" required>
            <mat-option *ngFor="let sector of data.sectors" [value]="sector.id">
              {{ sector.name }}
            </mat-option>
          </mat-select>
        </mat-form-field>

        <mat-form-field appearance="outline" class="full-width">
  <mat-label>Data de Entrada</mat-label>
  <input matInput [matDatepicker]="picker" formControlName="dateIn" required />
  <mat-datepicker-toggle matSuffix [for]="picker"></mat-datepicker-toggle>
  <mat-datepicker #picker></mat-datepicker>
</mat-form-field>
      </form>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button mat-dialog-close>Cancelar</button>
      <button mat-raised-button color="primary" [disabled]="!form.valid" (click)="onSubmit()">
        Confirmar
      </button>
    </mat-dialog-actions>
  `,
  styles: [`
    .full-width {
      width: 100%;
      margin-bottom: 16px;
    }
  `],
})
export class MoveToStageDialogComponent {
  form = new FormGroup({
    sectorId: new FormControl<number | null>(null),
    dateIn: new FormControl<string>(''),
  });

  constructor(
    public dialogRef: MatDialogRef<MoveToStageDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { sectors: Sector[]; orderId: number }
  ) {
    // Definir data atual como padrão
    this.form.patchValue({
      dateIn: new Date().toISOString().split('T')[0],
    });
  }

  onSubmit() {
    if (this.form.valid) {
      this.dialogRef.close(this.form.value);
    }
  }
}