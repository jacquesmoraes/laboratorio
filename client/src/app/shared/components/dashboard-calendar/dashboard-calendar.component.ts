import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CalendarOptions, EventInput } from '@fullcalendar/core';
import dayGridPlugin from '@fullcalendar/daygrid';
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
    plugins: [dayGridPlugin, interactionPlugin],
    timeZone: 'local',
    initialView: 'dayGridMonth',
    headerToolbar: {
      left: 'prev,next today',
      center: 'title',
      right: 'dayGridMonth,dayGridWeek,dayGridDay'
    },
    height: 'auto',
    dayMaxEventRows: true,
    eventDisplay: 'block',

    eventContent: function (arg) {
      const { event, view } = arg;
      const { clientName, patientName } = event.extendedProps;

      let content = '';

      if (view.type === 'dayGridMonth' || view.type === 'dayGridWeek') {
        content = `#${clientName} | ${patientName}`;
      } else {
        content = event.title; // título completo na visualização do dia
      }

      return {
        html: `<div class="fc-custom-content">${content}</div>`
      };
    },

    eventDidMount: (info) => {
      const el = info.el as HTMLElement;
      if (window.innerWidth < 768) {
        el.style.fontSize = '0.75rem';
        el.style.whiteSpace = 'normal';
        el.style.wordBreak = 'break-word';
      }
    },

    events: (fetchInfo, successCallback, failureCallback) => {
      const start = fetchInfo.startStr;
      const end = fetchInfo.endStr;

      this.scheduleService.getScheduleByRange(start, end).subscribe({
        next: (records: SectorScheduleRecord[]) => {
          const allEvents: EventInput[] = [];

          records.forEach(record => {
            record.deliveries.forEach(delivery => {
              const destino = delivery.deliveryType === 'FinalDelivery'
                ? 'entrega final'
                : delivery.deliveryType === 'TryIn'
                ? 'prova (try-in)'
                : delivery.deliveryType === 'SectorTransfer'
                ? `destino: ${delivery.targetSectorName ?? record.sectorName}`
                : 'desconhecido';

              const title = `#${delivery.orderNumber} | ${delivery.clientName.trim()} | ${delivery.patientName.trim()} | ${destino}`;

              let backgroundColor = '#276678';
              if (delivery.deliveryType === 'SectorTransfer') backgroundColor = '#96afb8';
              if (delivery.deliveryType === 'TryIn') backgroundColor = '#a288a9';
              if (delivery.isOverdue) backgroundColor = '#b873b8';

              const scheduledDay = new Date(delivery.scheduledDate);

              allEvents.push({
                title,
                start: scheduledDay,
                allDay: true,
                backgroundColor,
                borderColor: backgroundColor,
                textColor: '#ffffff',
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
