import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import {
  WebsiteCase,
  WebsiteCaseAdmin,
  CreateWebsiteCaseDto,
  UpdateWebsiteCaseDto,
  ReorderItem
} from '../models/website-case.interface';

@Injectable({
  providedIn: 'root'
})
export class WebsiteCaseService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/websitecases`;

  // Public endpoints
  getActiveForHomepage(): Observable<WebsiteCase[]> {
    return this.http.get<WebsiteCase[]>(`${this.apiUrl}/homepage`);
  }

  getDetailsById(id: number): Observable<WebsiteCase> {
    return this.http.get<WebsiteCase>(`${this.apiUrl}/${id}/details`);
  }

  // Admin endpoints
  getAll(): Observable<WebsiteCaseAdmin[]> {
    return this.http.get<WebsiteCaseAdmin[]>(this.apiUrl);
  }

  getActive(): Observable<WebsiteCaseAdmin[]> {
    return this.http.get<WebsiteCaseAdmin[]>(`${this.apiUrl}/active`);
  }

  getInactive(): Observable<WebsiteCaseAdmin[]> {
    return this.http.get<WebsiteCaseAdmin[]>(`${this.apiUrl}/inactive`);
  }

  getById(id: number): Observable<WebsiteCase> {
    return this.http.get<WebsiteCase>(`${this.apiUrl}/${id}`);
  }

  create(caseData: CreateWebsiteCaseDto): Observable<WebsiteCase> {
    return this.http.post<WebsiteCase>(this.apiUrl, caseData);
  }

  update(id: number, caseData: UpdateWebsiteCaseDto): Observable<WebsiteCase> {
    return this.http.put<WebsiteCase>(`${this.apiUrl}/${id}`, caseData);
  }

  activate(id: number): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/${id}/activate`, {});
  }

  deactivate(id: number): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/${id}/deactivate`, {});
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