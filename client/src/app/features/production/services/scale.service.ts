import { Injectable } from '@angular/core';
import { HttpClient, HttpContext } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { Scale, CreateScaleDto, UpdateScaleDto } from '../models/scale.interface';
import { SKIP_LOADER } from '../../../core/interceptors/skip-loader.token';

@Injectable({
  providedIn: 'root'
})
export class ScaleService {
  private apiUrl = `${environment.apiUrl}/scale`;

  constructor(private http: HttpClient) { }

  
  getAll(): Observable<Scale[]> {
    return this.http.get<Scale[]>(this.apiUrl);
  }

  getById(id: number): Observable<Scale> {
    return this.http.get<Scale>(`${this.apiUrl}/${id}`);
  }

  create(scale: CreateScaleDto): Observable<Scale> {
    return this.http.post<Scale>(`${this.apiUrl}`, scale, {
  context: new HttpContext().set(SKIP_LOADER, true)
});
  }

  update(id: number, scale: UpdateScaleDto): Observable<Scale> {
  return this.http.put<Scale>(`${this.apiUrl}/${id}`, scale, {
    context: new HttpContext().set(SKIP_LOADER, true)
  });
}

delete(id: number): Observable<void> {
  return this.http.delete<void>(`${this.apiUrl}/${id}`, {
    context: new HttpContext().set(SKIP_LOADER, true)
  });
}


}