import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { Shade, CreateShadeDto, UpdateShadeDto } from '../models/shade.interface';

@Injectable({
  providedIn: 'root'
})
export class ShadeService {
  private apiUrl = `${environment.apiUrl}/shade`;

  constructor(private http: HttpClient) { }

  getAll(scaleId?: number): Observable<Shade[]> {
    let url = this.apiUrl;
    if (scaleId) {
      url += `?scaleId=${scaleId}`;
    }
    return this.http.get<Shade[]>(url);
  }

  getById(id: number): Observable<Shade> {
    return this.http.get<Shade>(`${this.apiUrl}/${id}`);
  }

  create(shade: CreateShadeDto): Observable<Shade> {
    return this.http.post<Shade>(this.apiUrl, shade);
  }

  update(id: number, shade: UpdateShadeDto): Observable<Shade> {
    return this.http.put<Shade>(`${this.apiUrl}/${id}`, shade);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}