import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { catchError, Observable, of } from 'rxjs';

import {
  ScheduleDeliveryDto,
  ScheduleItemRecord,
  SectorScheduleRecord
} from '../models/schedule.model';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ScheduleService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/schedule`;

  /**
   * Agenda uma entrega para uma OS
   */
  scheduleDelivery(dto: ScheduleDeliveryDto): Observable<ScheduleItemRecord> {
      return this.http.post<ScheduleItemRecord>(this.apiUrl, dto);
  }

  
  updateSchedule(
    id: number,
    dto: ScheduleDeliveryDto
  ): Observable<ScheduleItemRecord> {
      return this.http.put<ScheduleItemRecord>(`${this.apiUrl}/${id}`, dto);
  }

 
  removeSchedule(id: number): Observable<void> {
    
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  /**
   * Retorna a agenda de hoje agrupada por setor
   */
  getTodaySchedule(): Observable<SectorScheduleRecord[]> {
    // ✅ GET para /api/schedule/today
    return this.http.get<SectorScheduleRecord[]>(`${this.apiUrl}/today`);
  }

getScheduleByRange(start: string, end: string) {
  return this.http.get<SectorScheduleRecord[]>(`${this.apiUrl}/range?start=${start}&end=${end}`);
}

  /**
   * Retorna a agenda de uma data específica agrupada por setor
   */
  getScheduleByDate(date: string): Observable<SectorScheduleRecord[]> {
    return this.http.get<SectorScheduleRecord[]>(
      `${this.apiUrl}/date/${date}`
    );
  }

  /**
   * Retorna a agenda do setor atual para uma data específica
   */
  getCurrentSectorSchedule(
    sectorId: number,
    date: string
  ): Observable<SectorScheduleRecord> {
    return this.http.get<SectorScheduleRecord>(
      `${this.apiUrl}/current-sector/${sectorId}/date/${date}`
    );
  }
  
  /**
 * Busca o agendamento ativo de uma OS
 */
getActiveScheduleByServiceOrder(serviceOrderId: number) {
  return this.http.get<ScheduleItemRecord>(`${this.apiUrl}/service-order/${serviceOrderId}`)
    .pipe(
      catchError(err => {
        if (err.status === 404) {
            return of(null as unknown as ScheduleItemRecord);
        }
        throw err;
      })
    );
}
}
