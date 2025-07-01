import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { WorkType, WorkTypeApiResponse, CreateWorkTypeDto, UpdateWorkTypeDto } from '../models/work-type.interface';

@Injectable({
  providedIn: 'root'
})
export class WorkTypeService {
  private apiUrl = `${environment.apiUrl}/workTypes`;

  constructor(private http: HttpClient) { }

  getAll(): Observable<WorkType[]> {
    return this.http.get<WorkTypeApiResponse[]>(this.apiUrl)
      .pipe(
        map(response => response.map(item => ({
          id: item.id,
          name: item.name,
          description: item.description,
          isActive: item.isActive,
          workSectionId: 0, // Não vem na resposta da API
          workSectionName: item.workSectionName
        })))
      );
  }

  getById(id: number): Observable<WorkType> {
    return this.http.get<WorkTypeApiResponse>(`${this.apiUrl}/${id}`)
      .pipe(
        map(response => ({
          id: response.id,
          name: response.name,
          description: response.description,
          isActive: response.isActive,
          workSectionId: 0, // Não vem na resposta da API
          workSectionName: response.workSectionName
        }))
      );
  }

  create(workType: CreateWorkTypeDto): Observable<WorkType> {
    return this.http.post<WorkTypeApiResponse>(this.apiUrl, workType)
      .pipe(
        map(response => ({
          id: response.id,
          name: response.name,
          description: response.description,
          isActive: response.isActive,
          workSectionId: 0,
          workSectionName: response.workSectionName
        }))
      );
  }

  update(id: number, workType: UpdateWorkTypeDto): Observable<WorkType> {
    return this.http.put<WorkTypeApiResponse>(`${this.apiUrl}/${id}`, workType)
      .pipe(
        map(response => ({
          id: response.id,
          name: response.name,
          description: response.description,
          isActive: response.isActive,
          workSectionId: 0,
          workSectionName: response.workSectionName
        }))
      );
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}