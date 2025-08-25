import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import {
  WebsiteWorkType,
  CreateWebsiteWorkTypeDto,
  UpdateWebsiteWorkTypeDto,
  ReorderItem
} from '../models/website-worktype.interface';

@Injectable({
  providedIn: 'root'
})
export class WebsiteWorkTypeService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/websiteworktypes`;

  // Public endpoints
  getActive(): Observable<WebsiteWorkType[]> {
    return this.http.get<WebsiteWorkType[]>(`${this.apiUrl}/active`);
  }

  // Admin endpoints
  getAll(): Observable<WebsiteWorkType[]> {
    return this.http.get<WebsiteWorkType[]>(this.apiUrl);
  }

  getById(id: number): Observable<WebsiteWorkType> {
    return this.http.get<WebsiteWorkType>(`${this.apiUrl}/${id}`);
  }

  create(workTypeData: CreateWebsiteWorkTypeDto): Observable<WebsiteWorkType> {
    return this.http.post<WebsiteWorkType>(this.apiUrl, workTypeData);
  }

  update(id: number, workTypeData: UpdateWebsiteWorkTypeDto): Observable<WebsiteWorkType> {
    return this.http.put<WebsiteWorkType>(`${this.apiUrl}/${id}`, workTypeData);
  }

  toggleActive(id: number): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/${id}/toggle-active`, {});
  }

  updateOrder(reorderItems: ReorderItem[]): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/reorder`, reorderItems);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}