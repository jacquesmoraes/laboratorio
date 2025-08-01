import { Component, ChangeDetectionStrategy, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ScheduledDeliveryType, ScheduleDeliveryDto, ScheduleItemRecord } from '../../../../core/models/schedule.model';
import { ScheduleService } from '../../../../core/services/schedule.service';

@Component({
  selector: 'app-schedule-delivery-modal',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatProgressSpinnerModule
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './schedule-delivery-modal.component.html',
  styleUrls: ['./schedule-delivery-modal.component.scss']
})
export class ScheduleDeliveryModalComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly scheduleService = inject(ScheduleService);
  private readonly dialogRef = inject(MatDialogRef<ScheduleDeliveryModalComponent>);
  private readonly snackBar = inject(MatSnackBar);
  
  private readonly data = inject(MAT_DIALOG_DATA) as {
    serviceOrderId: number;
    sectors: { sectorId: number; sectorName: string }[];
    scheduleId?: number; 
     existingSchedule?: ScheduleItemRecord; 
  };

  readonly deliveryTypes: ScheduledDeliveryType[] = ['SectorTransfer', 'TryIn', 'FinalDelivery'];
  isEditMode = false;
  loading = false;

  readonly form = this.fb.group({
    scheduledDate: ['', Validators.required],
    deliveryType: ['FinalDelivery', Validators.required],
    sectorId: [null as number | null]
  });

 ngOnInit() {
  // Se tem scheduleId e existingSchedule, é modo edição
  if (this.data.scheduleId && this.data.existingSchedule) {
    this.isEditMode = true;
    this.loadExistingScheduleData();
  } else {
    // Modo criação - definir data atual como padrão
    this.form.patchValue({
      scheduledDate: new Date().toISOString().split('T')[0],
    });
  }
}

  private loadExistingScheduleData() {
  this.loading = true;
  
  // Usar os dados passados diretamente
  const schedule = this.data.existingSchedule!;
  
  this.form.patchValue({
    scheduledDate: new Date(schedule.scheduledDate).toISOString().split('T')[0],
    deliveryType: schedule.deliveryType,
    sectorId: schedule.targetSectorName ? this.getSectorIdByName(schedule.targetSectorName) : null
  });
  
  this.loading = false;
}

  private getSectorIdByName(sectorName: string): number | null {
    const sector = this.sectors.find(s => s.sectorName === sectorName);
    return sector ? sector.sectorId : null;
  }

  showSector() {
    return this.form.get('deliveryType')?.value === 'SectorTransfer';
  }

 submit() {
  if (this.form.invalid || this.loading) return;

  const { scheduledDate, deliveryType, sectorId } = this.form.value;

  const dto: ScheduleDeliveryDto = {
    serviceOrderId: this.serviceOrderId,
    scheduledDate: scheduledDate ?? '',
    deliveryType: (deliveryType ?? 'FinalDelivery') as ScheduledDeliveryType,
    sectorId: sectorId ?? undefined
  };

  // Adicionar log para debug
  console.log('Enviando DTO:', dto);
  console.log('Modo edição:', this.isEditMode);
  console.log('Schedule ID:', this.data.scheduleId);

  this.loading = true;

  const request = this.isEditMode && this.data.scheduleId
    ? this.scheduleService.updateSchedule(this.data.scheduleId, dto)
    : this.scheduleService.scheduleDelivery(dto);

  request.subscribe({
    next: () => {
      this.loading = false;
      const message = this.isEditMode ? 'Agendamento atualizado com sucesso!' : 'Entrega agendada com sucesso!';
      this.snackBar.open(message, 'Fechar', { duration: 3000 });
      this.dialogRef.close(true);
    },
    error: (err) => {
      this.loading = false;
      console.error('Erro ao agendar entrega', err);
      console.error('Detalhes do erro:', err.error);
      this.snackBar.open('Erro ao agendar entrega', 'Fechar', { duration: 3000 });
    }
  });
}

  cancel() {
    this.dialogRef.close(false);
  }

  get serviceOrderId(): number {
    return this.data.serviceOrderId;
  }

  get sectors(): { sectorId: number; sectorName: string }[] {
    return this.data.sectors;
  }
}