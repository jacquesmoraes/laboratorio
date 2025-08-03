import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import {
  ClientUserListRecord,
  ClientUserDetailsRecord,
  ClientUserRegistrationResponse,
  RegisterClientUserRequest,
  UserManagementQueryParams,
  UserManagementPagination,
  BlockUserResponse,
  UnblockUserResponse,
  ResetAccessCodeResponse
} from '../models/user-management.interface';

@Injectable({
  providedIn: 'root'
})
export class UserManagementService {
  private http = inject(HttpClient);
 private authBaseUrl = 'https://localhost:7058/auth';

  // ========== LISTAGEM E DETALHES ==========

  /**
   * Obtém lista paginada de usuários client area
   */
  getClientUsers(params: UserManagementQueryParams): Observable<UserManagementPagination<ClientUserListRecord>> {
    const queryParams = new HttpParams()
      .set('pageNumber', params.pageNumber.toString())
      .set('pageSize', params.pageSize.toString())
      .set('sort', params.sort || 'clientName')
      .set('search', params.search || '');
    
    return this.http.get<UserManagementPagination<ClientUserListRecord>>(`${this.authBaseUrl}/client-users`, { 
      params: queryParams 
    });
  }

  /**
   * Obtém detalhes de um usuário específico
   */
getClientUserDetails(userId: string): Observable<ClientUserDetailsRecord> {
  return this.http.get<ClientUserDetailsRecord>(`${this.authBaseUrl}/client-users/${userId}`);
}

  // ========== REGISTRO DE USUÁRIOS ==========

  /**
   * Registra um novo usuário client area
   */
  registerClientUser(request: RegisterClientUserRequest): Observable<ClientUserRegistrationResponse> {
    return this.http.post<ClientUserRegistrationResponse>(`${this.authBaseUrl}/register-client`, request);
  }

  // ========== GERENCIAMENTO DE STATUS ==========

  /**
   * Bloqueia um usuário
   */
  blockClientUser(userId: string): Observable<BlockUserResponse> {
    return this.http.put<BlockUserResponse>(`${this.authBaseUrl}/client-users/${userId}/block`, {});
  }

  /**
   * Desbloqueia um usuário
   */
  unblockClientUser(userId: string): Observable<UnblockUserResponse> {
    return this.http.put<UnblockUserResponse>(`${this.authBaseUrl}/client-users/${userId}/unblock`, {});
  }

  // ========== GERENCIAMENTO DE CÓDIGO DE ACESSO ==========

  /**
   * Reseta o código de acesso de um usuário
   */
  resetClientUserAccessCode(userId: string): Observable<ResetAccessCodeResponse> {
    return this.http.post<ResetAccessCodeResponse>(`${this.authBaseUrl}/client-users/${userId}/reset-access-code`, {});
  }

  /**
   * Reenvia código de acesso por ClientId (método existente)
   */
  resendAccessCodeByClientId(clientId: number): Observable<ResetAccessCodeResponse> {
    return this.http.post<ResetAccessCodeResponse>(`${this.authBaseUrl}/resend-access-code/${clientId}`, {});
  }

  // ========== MÉTODOS AUXILIARES ==========

  /**
   * Constrói parâmetros de query para filtros avançados
   */
  buildQueryParams(params: Partial<UserManagementQueryParams>): HttpParams {
    let httpParams = new HttpParams()
      .set('pageNumber', (params.pageNumber || 1).toString())
      .set('pageSize', (params.pageSize || 10).toString());

    if (params.sort) {
      httpParams = httpParams.set('sort', params.sort);
    }

    if (params.search) {
      httpParams = httpParams.set('search', params.search);
    }

    if (params.isActive !== undefined) {
      httpParams = httpParams.set('isActive', params.isActive.toString());
    }

    if (params.hasValidAccessCode !== undefined) {
      httpParams = httpParams.set('hasValidAccessCode', params.hasValidAccessCode.toString());
    }

    return httpParams;
  }

  /**
   * Obtém estatísticas básicas dos usuários
   */
  getClientUsersStatistics(): Observable<{
    totalUsers: number;
    activeUsers: number;
    blockedUsers: number;
    usersWithValidAccessCode: number;
  }> {
    return this.http.get<{
      totalUsers: number;
      activeUsers: number;
      blockedUsers: number;
      usersWithValidAccessCode: number;
    }>(`${this.authBaseUrl}/client-users/statistics`);
  }
}