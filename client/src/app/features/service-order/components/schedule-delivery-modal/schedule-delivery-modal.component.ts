import { Component, ChangeDetectionStrategy, inject, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { ScheduledDeliveryType, ScheduleDeliveryDto } from '../../../../core/models/schedule.model';
import { ScheduleService } from '../../../../core/services/schedule.service';

@Component({
  selector: 'app-schedule-delivery-modal',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatButtonModule
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './schedule-delivery-modal.component.html',
  styleUrls: ['./schedule-delivery-modal.component.scss']
})
export class ScheduleDeliveryModalComponent {
  private readonly fb = inject(FormBuilder);
  private readonly scheduleService = inject(ScheduleService);
  private readonly dialogRef = inject(MatDialogRef<ScheduleDeliveryModalComponent>);
private readonly data = inject(MAT_DIALOG_DATA) as {
  serviceOrderId: number;
  sectors: { sectorId: number; sectorName: string }[];
};
 

  readonly deliveryTypes: ScheduledDeliveryType[] = ['SectorTransfer', 'TryIn', 'FinalDelivery'];

  readonly form = this.fb.group({
    scheduledDate: ['', Validators.required],
    deliveryType: ['FinalDelivery', Validators.required],
    sectorId: [null]
  });

  showSector() {
    return this.form.get('deliveryType')?.value === 'SectorTransfer';
  }

  submit() {
    if (this.form.invalid) return;

    const { scheduledDate, deliveryType, sectorId } = this.form.value;

    const dto: ScheduleDeliveryDto = {
      serviceOrderId: this.serviceOrderId,
      scheduledDate: scheduledDate ?? '',
      deliveryType: (deliveryType ?? 'FinalDelivery') as ScheduledDeliveryType,
      sectorId: sectorId ?? undefined
    };

    this.scheduleService.scheduleDelivery(dto).subscribe({
      next: () => this.dialogRef.close(true),
      error: (err) => console.error('Erro ao agendar entrega', err)
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
