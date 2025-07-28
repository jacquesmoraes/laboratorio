import { Injectable } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';

interface ApiErrorResponse {
  message?: string;
  error?: string;
  errors?: string[];
  statusCode?: number;
}

interface ServiceOrderError {
  error?: ApiErrorResponse;
  message?: string;
  status?: number;
}

@Injectable({
  providedIn: 'root'
})
export class ErrorMappingService {
  
  mapServiceOrderError(error: ServiceOrderError | HttpErrorResponse | string): string {
    if (typeof error === 'string') {
      return error;
    }

    // Extrair mensagem do erro
    const errorMessage = this.extractErrorMessage(error);

    // Mapear mensagens específicas do backend
    if (errorMessage.includes('não há estágio aberto')) {
      return 'Não há estágio aberto para enviar para prova.';
    }
    
    if (errorMessage.includes('já está finalizada')) {
      return 'Não é possível alterar uma ordem finalizada.';
    }
    
    if (errorMessage.includes('já está em prova')) {
      return 'A ordem já está em prova.';
    }
    
    if (errorMessage.includes('data inválida') || errorMessage.includes('must occur after')) {
      return 'Data inválida. Verifique a data informada.';
    }
    
    if (errorMessage.includes('not found')) {
      return 'Ordem de serviço não encontrada.';
    }
    
    if (errorMessage.includes('same client')) {
      return 'Todas as ordens devem ser do mesmo cliente.';
    }
    
    if (errorMessage.includes('No valid service order found')) {
      return 'Nenhuma ordem válida encontrada para finalizar.';
    }

    // Se não encontrou mapeamento específico, retorna a mensagem original
    return errorMessage || 'Ocorreu um erro inesperado.';
  }

  private extractErrorMessage(error: ServiceOrderError | HttpErrorResponse): string {
    // Se é HttpErrorResponse
    if (error instanceof HttpErrorResponse) {
      if (error.error) {
        const apiError = error.error as ApiErrorResponse;
        return apiError.message || apiError.error || '';
      }
      return error.message || '';
    }

    // Se é ServiceOrderError
    if (error.error) {
      const apiError = error.error as ApiErrorResponse;
      return apiError.message || apiError.error || '';
    }

    return error.message || '';
  }
}