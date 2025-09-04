// ========== REQUEST INTERFACES ==========

export interface RegisterClientUserRequest {
  clientId: number;
  displayName: string;
  email: string;
  // ✅ REMOVER: Campos de senha desnecessários
  // password: string;
  // confirmPassword: string;
}

// ========== RESPONSE INTERFACES ==========

export interface ClientUserRecord {
  userId: string;
  email: string;
  displayName: string;
  role: string;
  clientId: number;
  isFirstLogin: boolean;
  accessCode: string;
}

export interface ClientUserRegistrationResponse {
  expiresAt: string;
  user: ClientUserRecord;
}

export interface ClientUserListRecord {
  userId: string;
  clientId: number;
  clientName: string;
  email: string;
  isActive: boolean;
  createdAt: string;
  lastLoginAt?: string;
  hasValidAccessCode: boolean;
}

export interface ClientUserDetailsRecord {
  // Dados da listagem
  userId: string; 
  clientId: number;
  clientName: string;
  email: string;
  isActive: boolean;
  isFirstLogin: boolean;
  createdAt: string;
  lastLoginAt?: string;
  hasValidAccessCode: boolean;
  
  // Dados adicionais para detalhes
  accessCode?: string;
  accessCodeExpiresAt?: string;
  isAccessCodeValid: boolean;
  loginHistory?: LoginHistoryRecord[];
}

export interface LoginHistoryRecord {
  loginAt: string;
  ipAddress: string;
  userAgent: string;
}

// ========== QUERY PARAMS ==========

export interface UserManagementQueryParams {
  pageNumber: number;
  pageSize: number;
  sort?: string;
  search?: string;
  isActive?: boolean;
  hasValidAccessCode?: boolean;
}

// ========== PAGINATION ==========

export interface UserManagementPagination<T> {
  pageNumber: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
  data: T[];
}

// ========== ACTION RESPONSES ==========

export interface BlockUserResponse {
  message: string;
}

export interface UnblockUserResponse {
  message: string;
}

export interface ResetAccessCodeResponse {
  accessCode: string;
}

// ========== ENUMS ==========

export enum UserStatus {
  Active = 'active',
  Blocked = 'blocked'
}

export enum AccessCodeStatus {
  Valid = 'valid',
  Expired = 'expired',
  None = 'none'
}

// ========== FILTER OPTIONS ==========

export interface UserManagementFilters {
  search?: string;
  isActive?: boolean;
  hasValidAccessCode?: boolean;
  sortBy?: 'email' | 'clientName' | 'createdAt' | 'lastLoginAt';
  sortOrder?: 'asc' | 'desc';
}