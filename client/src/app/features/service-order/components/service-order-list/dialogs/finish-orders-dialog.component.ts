import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-finish-orders-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
  ],
  template: `
    <h2 mat-dialog-title>Finalizar Ordens de Serviço</h2>
    <mat-dialog-content>
      <div class="dialog-content">
        <p class="summary-text">
          Você está prestes a finalizar <strong>{{ data.orderCount }}</strong> ordem(ns) de serviço do cliente 
          <strong>{{ data.clientName }}</strong>.
        </p>
        
        <form [formGroup]="form">
          <mat-form-field appearance="outline" class="full-width">
            <mat-label>Data de Finalização</mat-label>
            <input matInput type="date" formControlName="dateOut" required />
            <mat-hint>Data em que as ordens foram finalizadas</mat-hint>
          </mat-form-field>
        </form>
        
        <div class="warning-section">
          <mat-icon class="warning-icon">warning</mat-icon>
          <div class="warning-text">
            <strong>Atenção:</strong> Esta ação não pode ser desfeita. 
            As ordens serão marcadas como finalizadas e não poderão mais ser editadas.
          </div>
        </div>
      </div>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button mat-dialog-close>Cancelar</button>
      <button 
        mat-raised-button 
        color="primary" 
        [disabled]="!form.valid" 
        (click)="onSubmit()"
        class="finish-button">
        <mat-icon>check</mat-icon>
        Finalizar
      </button>
    </mat-dialog-actions>
  `,
  styles: [`
    .dialog-content {
      min-width: 400px;
    }
    
    .summary-text {
      margin-bottom: 20px;
      line-height: 1.5;
    }
    
    .full-width {
      width: 100%;
      margin-bottom: 20px;
    }
    
    .warning-section {
      display: flex;
      align-items: flex-start;
      gap: 12px;
      padding: 16px;
      background-color: #fff3e0;
      border: 1px solid #ffb74d;
      border-radius: 8px;
      margin-top: 16px;
    }
    
    .warning-icon {
      color: #f57c00;
      margin-top: 2px;
    }
    
    .warning-text {
      color: #e65100;
      font-size: 14px;
      line-height: 1.4;
    }
    
    .finish-button {
      background-color: #276678;
    }
    
    .finish-button:hover {
      background-color: #334a52;
    }
  `],
})
export class FinishOrdersDialogComponent {
  form = new FormGroup({
    dateOut: new FormControl<string>(''),
  });

  constructor(
    public dialogRef: MatDialogRef<FinishOrdersDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { orderCount: number; clientName: string }
  ) {
    // Definir data atual como padrão
    this.form.patchValue({
      dateOut: new Date().toISOString().split('T')[0],
    });
  }

  onSubmit() {
    if (this.form.valid) {
      this.dialogRef.close(this.form.value);
    }
  }
}