import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, of } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface ViaCepResponse {
  cep: string;
  logradouro: string;
  complemento: string;
  unidade: string;
  bairro: string;
  localidade: string;
  uf: string;
  estado: string;
  regiao: string;
  ibge: string;
  gia: string;
  ddd: string;
  siafi: string;
  erro?: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class CepService {
  private http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/cep`;

  searchCep(cep: string): Observable<ViaCepResponse | null> {
    // Remove caracteres não numéricos
    const cleanCep = cep.replace(/\D/g, '');
    
    if (cleanCep.length !== 8) {
      return of(null);
    }

    return this.http.get<ViaCepResponse>(`${this.apiUrl}/${cleanCep}`)
      .pipe(
        catchError(() => of(null))
      );
  }
}