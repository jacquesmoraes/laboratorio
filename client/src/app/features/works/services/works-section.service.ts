import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map, tap, catchError } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { WorkSection, WorkSectionApiResponse } from '../models/work-section.interface';

@Injectable({
  providedIn: 'root'
})
export class WorkSectionService {
  private apiUrl = `${environment.apiUrl}/workSections`;

  constructor(private http: HttpClient) { }

  getAll(): Observable<WorkSection[]> {
    
    return this.http.get<WorkSectionApiResponse[]>(this.apiUrl)
      .pipe(
        
        map(response => response.map(item => ({
          id: item.id,
          name: item.name
        }))),
        catchError(error => {
          console.error('WorkSectionService: Erro na requisição:', error);
          throw error;
        })
      );
  }

  getById(id: number): Observable<WorkSection> {
    return this.http.get<WorkSectionApiResponse>(`${this.apiUrl}/${id}`)
      .pipe(
        map(response => ({
          id: response.id,
          name: response.name
        }))
      );
  }

  create(workSection: Omit<WorkSection, 'id'>): Observable<WorkSection> {
    return this.http.post<WorkSectionApiResponse>(this.apiUrl, workSection)
      .pipe(
        map(response => ({
          id: response.id,
          name: response.name
        }))
      );
  }

  update(id: number, workSection: Omit<WorkSection, 'id'>): Observable<WorkSection> {
    return this.http.put<WorkSectionApiResponse>(`${this.apiUrl}/${id}`, workSection)
      .pipe(
        map(response => ({
          id: response.id,
          name: response.name
        }))
      );
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}