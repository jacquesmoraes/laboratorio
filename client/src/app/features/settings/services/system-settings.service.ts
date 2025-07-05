import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { SystemSettings, UpdateSystemSettingsDto } from '../models/system.interface';


@Injectable({
  providedIn: 'root'
})
export class SettingsService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/settings`;

  getSettings(): Observable<SystemSettings> {
    return this.http.get<SystemSettings>(this.apiUrl).pipe(
      map(settings => ({
        ...settings,
        // Corrigir a construção da URL da logo
        logoUrl: settings.logoUrl 
          ? `${environment.apiUrl.replace('/api', '')}${settings.logoUrl}` 
          : undefined
      }))
    );
  }

  updateSettings(dto: UpdateSystemSettingsDto): Observable<void> {
    return this.http.put<void>(this.apiUrl, dto);
  }

  uploadLogo(file: File): Observable<{ success: boolean; fileName: string; url: string }> {
    const formData = new FormData();
    formData.append('file', file);
    
    return this.http.post<{ success: boolean; fileName: string; url: string }>(
      `${this.apiUrl}/logo`,
      formData
    );
  }
}