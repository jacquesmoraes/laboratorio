import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CalendarOptions, EventInput } from '@fullcalendar/core';
import dayGridPlugin from '@fullcalendar/daygrid';
import timeGridPlugin from '@fullcalendar/timegrid';
import interactionPlugin from '@fullcalendar/interaction';
import { FullCalendarModule } from '@fullcalendar/angular';
import { SectorScheduleRecord } from '../../../core/models/schedule.model';
import { ScheduleService } from '../../../core/services/schedule.service';

@Component({
  selector: 'app-dashboard-calendar',
  standalone: true,
  imports: [CommonModule, FullCalendarModule],
  templateUrl: './dashboard-calendar.component.html',
  styleUrls: ['./dashboard-calendar.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class DashboardCalendarComponent {
  private readonly scheduleService = inject(ScheduleService);

  readonly calendarOptions = signal<CalendarOptions>({
    plugins: [dayGridPlugin, timeGridPlugin, interactionPlugin],
     timeZone: 'local',
    initialView: 'timeGridWeek',
    headerToolbar: {
      left: 'prev,next today',
      center: 'title',
      right: 'dayGridMonth,timeGridWeek,timeGridDay'
    },
    eventColor: '#276678',
    height: 'auto',

    dayMaxEvents: false,
    eventDisplay: 'block',
    slotMinTime: '07:00:00',
    slotMaxTime: '20:00:00',
    slotDuration: '01:00:00',

    slotLabelFormat: {
      hour: '2-digit',
      minute: '2-digit',
      hour12: false
    },

    eventTimeFormat: {
      hour: '2-digit',
      minute: '2-digit',
      hour12: false
    },

    events: (fetchInfo, successCallback, failureCallback) => {
      const start = fetchInfo.startStr;
      const end = fetchInfo.endStr;

      this.scheduleService.getScheduleByRange(start, end).subscribe({
        next: (records: SectorScheduleRecord[]) => {
          const allEvents: EventInput[] = [];

          records.forEach(record => {
            let offset = 0; // para distribuir os horários no mesmo dia

            record.deliveries.forEach(delivery => {
              let destino = '';

              if (delivery.deliveryType === 'FinalDelivery') {
                destino = 'entrega final';
              } else if (delivery.deliveryType === 'TryIn') {
                destino = 'prova (try-in)';
              } else if (delivery.deliveryType === 'SectorTransfer') {
                destino = `destino: ${delivery.targetSectorName ?? record.sectorName}`;
              } else {
                destino = 'desconhecido';
              }

              const title = `#${delivery.orderNumber} | ${delivery.clientName.trim()} | ${delivery.patientName.trim()} | ${destino}`;

              let backgroundColor = '#276678';
              if (delivery.deliveryType === 'SectorTransfer') backgroundColor = '#96afb8';
              if (delivery.deliveryType === 'TryIn') backgroundColor = '#a288a9';
              if (delivery.isOverdue) backgroundColor = '#b873b8';

              const eventStart = new Date(delivery.scheduledDate);
              eventStart.setHours(9 + offset, 0, 0, 0);

              const eventEnd = new Date(eventStart);
              eventEnd.setHours(eventStart.getHours() + 1);

              offset++; // próximo evento no mesmo setor no mesmo dia sobe para a hora seguinte

              allEvents.push({
                title,
                start: eventStart,
                end: eventEnd,
                backgroundColor,
                borderColor: backgroundColor,
                textColor: '#ffffff',
                allDay: false,
                extendedProps: {
                  orderNumber: delivery.orderNumber,
                  patientName: delivery.patientName,
                  clientName: delivery.clientName,
                  deliveryType: delivery.deliveryType,
                  currentSector: delivery.currentSectorName,
                  targetSector: delivery.targetSectorName,
                  status: delivery.status
                }
              });
            });
          });

          successCallback(allEvents);
        },
        error: err => {
          console.error('Erro ao carregar agenda', err);
          failureCallback(err);
        }
      });
    }
  });
}
